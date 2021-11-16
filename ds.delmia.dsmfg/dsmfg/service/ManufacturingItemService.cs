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

using ds.authentication;
using ds.delmia.dsmfg.converter;
using ds.delmia.dsmfg.exception;
using ds.delmia.dsmfg.fields;
using ds.delmia.dsmfg.mask;
using ds.delmia.dsmfg.model;
using ds.delmia.dsmfg.model.process;
using ds.delmia.model;
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.service;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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
        public async Task<List<NlsLabeledItemSet<ManufacturingItem>>> SearchAll(SearchQuery _searchString)
        {
            return await SearchUtils<ManufacturingItem>.SearchAllAsync(this, _searchString);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItem>> SearchAsync(SearchQuery _searchQuery, long _skip, long _top, string _mask = null)
        {
            string searchManufacturingItems = string.Format("{0}{1}{2}", GetBaseResource(), MANUFACTURING_ITEM, SEARCH);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", HttpUtility.UrlEncode(_searchQuery.GetSearchString()));

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItem>>();
        }

        //Gets a indexed search result of Manufacturing Item
        public async Task<NlsLabeledItemSet<ManufacturingItem>> Search(SearchQuery _searchQuery, long _skip = 0, long _top = 100)
        {
            return await SearchAsync(_searchQuery, _skip, _top);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItemDetails>> SearchWithDetails(SearchQuery _searchQuery, long _skip = 0, long _top = 100)
        {
            string searchManufacturingItems = string.Format("{0}{1}{2}", GetBaseResource(), MANUFACTURING_ITEM, SEARCH);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Details;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", _searchQuery.GetSearchString());

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItemDetails>>(deserializeOptions);
        }
        #endregion

        public async Task<NlsLabeledItemSet<ManufacturingItem>> CreateManufacturingItem(ManufacturingItemCreate _mfgItem)
        {
            ManufacturingModelSetCreate itemsCreate = new ManufacturingModelSetCreate();
            itemsCreate.items.Add(_mfgItem);

            return await CreateManufacturingItem(itemsCreate);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItem>> CreateManufacturingItem(ManufacturingModelSetCreate _mfgItemSet)
        {
            //.................

            string createManufacturingItemEndpoint = string.Format("{0}{1}", GetBaseResource(), MANUFACTURING_ITEM);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());

            string mfgItemSetPayload = JsonSerializer.Serialize(_mfgItemSet);
            HttpResponseMessage requestResponse = await PostAsync(createManufacturingItemEndpoint, null, null, mfgItemSetPayload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItem>>(deserializeOptions);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItem>> GetManufacturingItem(string _mfgItemId)
        {
            string searchManufacturingItems = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItem>>(deserializeOptions);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItem>> GetManufacturingItemFields(string _mfgItemId, ManufacturingItemFields _mfgItemFields)
        {
            string searchManufacturingItems = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());
            queryParams.Add("$fields", _mfgItemFields.ToString());

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItem>>(deserializeOptions);
        }

        public async Task<NlsLabeledItemSet<ManufacturingItemDetails>> GetManufacturingItemFieldsDetails(string _mfgItemId, ManufacturingItemFields _fields)
        {
            string getManufacturingItem = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

            ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Details;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());
            queryParams.Add("$fields", _fields.ToString());
            
            HttpResponseMessage requestResponse = await GetAsync(getManufacturingItem, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItemDetails>>(deserializeOptions);

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
            ScopeEngItemMask mfgItemMask = ScopeEngItemMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemMask.GetString());

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
        public async Task<NlsLabeledItemSet<ManufacturingInstance>> GetManufacturingItemInstances(string _mfgItemId)
        {
            string getManufacturingInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, MANUFACTURING_INSTANCE );

            ManufacturingItemInstanceMask mfgItemInstanceMask = ManufacturingItemInstanceMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemInstanceMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingInstance>>();
        }

        public async Task<NlsLabeledItemSet<ManufacturingInstanceReference>> GetManufacturingItemInstancesWithReference(string _mfgItemId)
        {
            string getManufacturingInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId, MANUFACTURING_INSTANCE);

            ManufacturingItemInstanceMask mfgItemInstanceMask = ManufacturingItemInstanceMask.Details;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgItemInstanceMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingInstanceReference>>();

        }


        #endregion
    }
}
