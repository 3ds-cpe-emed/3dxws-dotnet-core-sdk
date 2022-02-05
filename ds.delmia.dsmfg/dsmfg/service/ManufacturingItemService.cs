//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//------------------------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using ds.authentication;

using ds.enovia.service;

using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.common.serialization;

using ds.delmia.dsmfg.exception;
using ds.delmia.dsmfg.model;
using ds.delmia.dsmfg.schema;
using ds.enovia.common.interfaces.attributes;
using System.Text.Json.Serialization;
using ds.delmia.model.collections;
using ds.delmia.dsmfg.model.reference;
using ds.delmia.dsmfg.interfaces;

namespace ds.delmia.dsmfg.service
{
    public class ManufacturingItemService : EnoviaBaseService, I3DXSearchable<ManufacturingItem>
    {
        private const string ENG_ITEM_TYPE = "dseng:EngItem";

        private const string BASE_RESOURCE = "/resources/v1/modeler/dsmfg";
        private const string SEARCH = "/search";
        private const string MANUFACTURING_ITEM = "/dsmfg:MfgItem";
        private const string MANUFACTURING_INSTANCE= "/dsmfg:MfgItemInstance";
        private const string RESULTING_ENG_ITEM = "/dsmfg:ResultingEngItem";
        private const string SCOPE_ENG_ITEM = "/dsmfg:ScopeEngItem";
        private const string SCOPE_ITEM_ATTACH = SCOPE_ENG_ITEM + "/attach";
        private const string SCOPE_ITEM_DETACH = SCOPE_ENG_ITEM + "/detach";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public ManufacturingItemService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        #region dsmfg:ManufacturingItem

        #region Search
        //<summary>Notes from public documentation
        // Engineering Web Services 1.3.0 - Gets a list of Engineering Items. By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
        // Recommendation: Use $searchStr query parameter with a minimum of two characters for better performances.</summary>
        public async Task<List<NlsLabeledItemSet2<ManufacturingItem>>> SearchAll(SearchQuery _searchString)
        {
            return await SearchUtils<ManufacturingItem>.SearchAllAsync(this, _searchString);
        }

        public async Task<NlsLabeledItemSet2<ManufacturingItem>> SearchAsync(SearchQuery _searchQuery, long _skip, long _top, string _mask = null)
        {
            string searchManufacturingItems = string.Format("{0}{1}{2}", GetBaseResource(), MANUFACTURING_ITEM, SEARCH);

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT);
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", HttpUtility.UrlEncode(_searchQuery.GetSearchString()));

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            //TODO: Review this to use ManufacturingItemRead
            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet2<ManufacturingItem>>();
        }

        //Gets a indexed search result of Manufacturing Item
        public async Task<NlsLabeledItemSet2<ManufacturingItem>> Search(SearchQuery _searchQuery, long _skip = 0, long _top = 100)
        {
            return await SearchAsync(_searchQuery, _skip, _top);
        }
        #endregion

        public async Task<IManufacturingItem> CreateManufacturingItem(IManufacturingItemBaseCreate _mfgItem, string _maskName = MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT)
        {
            // input validation
            if (_mfgItem == null) throw new ArgumentNullException();

            // Prepare Request ---

            string requestUri = string.Format("{0}{1}", GetBaseResource(), MANUFACTURING_ITEM);

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", _maskName);

            string mfgItemSetPayload = SerializeManufacturingItemSetRequest(_mfgItem);

            // Send request
            HttpResponseMessage response = await PostAsync(requestUri, _body: mfgItemSetPayload, _queryParameters: queryParams);

            // Handle response
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(response));
            }

            // Deserialize response
            NlsLabeledItemSet2<ManufacturingItem> returnSet = await DeserializeResponse<NlsLabeledItemSet2<ManufacturingItem>>(response, _maskName);

            if ((null == returnSet) || (returnSet.member.Count != 1))
            {
                throw (new ManufacturingResponseException(response));
            }

            return returnSet.member[0];
        }

        public string SerializeManufacturingItemSetRequest(object _inputData)
        {
            ManufacturingItemSetCreate itemsCreate = new ManufacturingItemSetCreate();
            itemsCreate.items.Add(_inputData);

            Type inputDataType = _inputData.GetType();

            if (inputDataType.IsSubclassOf(typeof(JsonSerializable)))
            {
                JsonSerializerOptions serializationOptions = GetSerializationOptions(inputDataType);
                return JsonSerializer.Serialize(itemsCreate, serializationOptions);
            }


            return JsonSerializer.Serialize(itemsCreate);
        }

        private JsonSerializerOptions GetSerializationOptions(Type t)
            {
            JsonConverter jconverter = GetJsonSerializableConverter(t);
            JsonSerializerOptions __options = new JsonSerializerOptions();
            __options.Converters.Add(jconverter);
            return __options;
        }

        private JsonConverter GetJsonSerializableConverter(Type t)
        {
            Type[] typeParams = new Type[] { t };
            Type constructedType = typeof(JsonSerializableConverter<>).MakeGenericType(typeParams);
            return (JsonConverter)(Activator.CreateInstance(constructedType));
        }

        public async Task<NlsLabeledItemSet2<ManufacturingItem>> GetManufacturingItem(string _mfgItemId, string _maskName = MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT)
        {
            // Prepare Request
            string requestUri = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

            // mask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", _maskName);

            // Send Request
            HttpResponseMessage response = await GetAsync(requestUri, queryParams);

            // Handle Response
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(response));
            }

            return await DeserializeResponse<NlsLabeledItemSet2<ManufacturingItem>>(response, _maskName);
        }
        #endregion

        #region in development

        public async Task<NlsLabeledItemSet2<ManufacturingItem>> GetManufacturingItem2(IUniqueIdentifier _mfgItemId, string _maskName = MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT, IList<string> _includeFieldsNames = null)
        {
            return await GetManufacturingItem2(_mfgItemId.Id, _maskName, _includeFieldsNames);
        }

        public async Task<NlsLabeledItemSet2<ManufacturingItem>> GetManufacturingItem2(string _mfgItemId, string _maskName = MFGResourceNames.DSMFG_MFGITEM_MASK_DEFAULT, IList<string> _includeFieldsNames = null)
        {
            if (_mfgItemId == null) throw new ArgumentNullException("_mfgItemId is required");

            //Build the Request

            string requestUri = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

            //mask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            if (_maskName != null)
                queryParams.Add("$mask", _maskName);

            //fields
            string schemaFieldsQuery = SchemaUtils.GetFieldsQueryString(_includeFieldsNames);
            if (schemaFieldsQuery != null)
                queryParams.Add("$fields", schemaFieldsQuery);

            //Send the Request
            HttpResponseMessage response = await GetAsync(requestUri, queryParams);

            //Handle the Response
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // handle according to established exception policy
                throw (new ManufacturingResponseException(response));
            }

            //deserialize

            return await DeserializeResponse<NlsLabeledItemSet2<ManufacturingItem>>(response, _maskName, _includeFieldsNames);
            
        }

        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response, string maskName, IList<string> fieldsList = null)
        {
            Json3DXSchemaConverter<ManufacturingItem> jsonConverter = new DefaultMaskSchemaConverter<ManufacturingItem>(maskName);
        
            // Decorate mask with fields
            if ((fieldsList != null) && (fieldsList.Count > 0))
            {
                foreach (string fieldsName in fieldsList)
                {
                    jsonConverter = new DefaultFieldsSchemaConverter<ManufacturingItem>(fieldsName, jsonConverter);
                }
            }

            var deserializeOptions = new JsonSerializerOptions();
            
            deserializeOptions.Converters.Add(jsonConverter);

            return await response.Content.ReadFromJsonAsync<T>(deserializeOptions);
        }
        #endregion

        #region Scope Link 

        public async Task<bool> CreateScopeLink(string _mfgItemId, string _engItemId)
        {
            //.................

            string createScopeItemLinkResourceUri = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, SCOPE_ITEM_ATTACH);

            // Specification (1.3.0) does not specify any mask
            //ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            //// masks
            //Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("$mask", mfgItemMask.GetString());
            
            BusinessObjectIdentifier engItemBOId = new BusinessObjectIdentifier();

            engItemBOId.identifier = _engItemId;
            engItemBOId.type = "VPMReference"; //this is not required according to the schema in the specification
            engItemBOId.source = "3DSpace";
            engItemBOId.relativePath = $"/resources/v1/modeler/dseng/{ENG_ITEM_TYPE}/{_engItemId}";

            string engItemPayload = JsonSerializer.Serialize(engItemBOId);
            HttpResponseMessage requestResponse = await PostAsync(createScopeItemLinkResourceUri, null, null, engItemPayload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return true;
        }

        public async Task<bool> DeleteScopeLink(string _mfgItemId, string _engItemId)
        {
            //.................

            string createScopeItemLinkResourceUri = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, SCOPE_ITEM_DETACH);

            // Specification (1.3.0) does not specify any mask
            //ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            //// masks
            //Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("$mask", mfgItemMask.GetString());

            BusinessObjectIdentifier engItemBOId = new BusinessObjectIdentifier();

            engItemBOId.identifier = _engItemId;
            engItemBOId.type = "VPMReference"; //this is not required according to the schema in the specification
            engItemBOId.source = GetBaseResource();
            engItemBOId.relativePath = $"/resources/v1/modeler/dseng/{ENG_ITEM_TYPE}/{_engItemId}";

            string engItemPayload = JsonSerializer.Serialize(engItemBOId);
            HttpResponseMessage requestResponse = await PostAsync(createScopeItemLinkResourceUri, null, null, engItemPayload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return true;
        }

        public async Task<NlsLabeledItemSet<ScopeEngItemLink>> GetScopeLink(string _mfgItemId)
        {
            //.................

            string getScopeItemLinkResourceUri = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, SCOPE_ENG_ITEM);

            // Specification (1.3.0) does not specify any mask
            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", MFGResourceNames.DSMFG_SCOPEENGITEM_MASK_DEFAULT);

            HttpResponseMessage requestResponse = await GetAsync(getScopeItemLinkResourceUri);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ScopeEngItemLink>>();

        }
        #endregion

        #region dsmfg:ManufacturingInstance

        // Gets all the Manufacturing Item Instances
        public async Task<NlsLabeledItemSet2<ManufacturingInstance>> GetManufacturingItemInstances(string _mfgItemId)
        {
            string getManufacturingInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, MANUFACTURING_INSTANCE );

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", MFGResourceNames.DSMFG_MFGINST_MASK_DEFAULT);

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet2<ManufacturingInstance>>();
        }

        public async Task<NlsLabeledItemSet2<ManufacturingInstanceReference>> GetManufacturingItemInstancesWithReference(string _mfgItemId)
        {
            string getManufacturingInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, MANUFACTURING_INSTANCE);

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", MFGResourceNames.DSMFG_MFGINST_MASK_DETAILS);

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet2<ManufacturingInstanceReference>>();

        }

        #endregion
    }
}