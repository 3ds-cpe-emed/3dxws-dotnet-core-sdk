﻿//------------------------------------------------------------------------------------------------------------------------------------
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
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dseng.exception;
using ds.enovia.dseng.fields;
using ds.enovia.dseng.mask;
using ds.enovia.dseng.model;
using ds.enovia.dseng.model.configured;
using ds.enovia.dseng.model.filterable;
using ds.enovia.dseng.search;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace ds.enovia.dseng.service
{
    public class EngineeringServices : EnoviaBaseService, I3DXSearchable<EngineeringItem>
    {
        //example: "/resources/v1/modeler/dseng/dseng:EngItem/search?tenant={{tenant}}&$searchStr={{search-string}}&$mask=dsmveng:EngItemMask.Details&$top=1000&$skip=0"
        private const string BASE_RESOURCE = "/resources/v1/modeler/dseng/dseng:EngItem";
        private const string SEARCH = "/search";
        private const string ENTERPRISE_REFERENCE = "/dseng:EnterpriseReference";
        private const string CONFIGURED = "/dscfg:Configured";
        private const string ENGINEERING_INSTANCES = "/dseng:EngInstance";
        private const string FILTERABLE = "/dscfg:Filterable";
        
        // Notes from public documentation
        // Engineering Web Services 1.3.0 - Gets a list of Engineering Items.
        // By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
        private const long MAX_VALS_PER_QUERY = 1000;

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public EngineeringServices (string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }
        #region Search
        //<summary>Notes from public documentation
        // Engineering Web Services 1.3.0 - Gets a list of Engineering Items. By default, returns a total of up to 50 items, can be optionally increased upto 1000 items using $top query parameter.
        // Recommendation: Use $searchStr query parameter with a minimum of two characters for better performances.</summary>
        public async Task<List<NlsLabeledItemSet<EngineeringItem>>> SearchAll(SearchQuery _searchString, EngineeringSearchMask _mask = EngineeringSearchMask.Default, long _top = MAX_VALS_PER_QUERY)
        {
            return await SearchUtils<EngineeringItem>.SearchAllAsync(this, _searchString);
        }

        public async Task<NlsLabeledItemSet<EngineeringItem>> SearchAsync(SearchQuery _searchString, long _skip, long _top, string _mask = null)
        {
            return await _Search(_searchString.GetSearchString(), _skip, _top);
        }

        public async Task<NlsLabeledItemSet<EngineeringItem>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, string _mask = null)
        {
            return await SearchAsync(_searchString, _skip, _top, _mask);
        }

        //Important: Queries must not exceed 4096 characters.
        private async Task<NlsLabeledItemSet<EngineeringItem>> _Search(string _searchString, long _skip = 0, long _top = 100, EngineeringSearchMask _mask = EngineeringSearchMask.Default)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", _mask.GetString());
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", HttpUtility.UrlEncode(_searchString));

            string searchResource = string.Format("{0}{1}", GetBaseResource(), SEARCH);

            HttpResponseMessage requestResponse = await GetAsync(searchResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                //throw (new DerivedOutputException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItem>>();
        }
        #endregion

        #region Create Engineering Item
        public async Task<NlsLabeledItemSet<EngineeringItem>> CreateEngineeringItem(EngineeringItemCreate _engItem, bool _multivaluated = false)
        {
            CreateItemSet itemsCreate = new CreateItemSet();
            List<object> items = new List<object>()
            {
                _engItem
            };
            itemsCreate.items = items;

            return await CreateEngineeringItems(itemsCreate, _multivaluated);
        }

        public async Task<NlsLabeledItemSet<EngineeringItem>> CreateEngineeringItems(CreateItemSet _engItemSet, bool _multivaluated = false)
        {
            string createEngineeringItemEndpoint = string.Format("{0}", GetBaseResource());

            EngineeringSearchMask engItemMask = EngineeringSearchMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", engItemMask.GetString());
            if (_multivaluated)
            {
                queryParams.Add("$mva", "true");
            }

            string engItemSetPayload = JsonSerializer.Serialize(_engItemSet);
            HttpResponseMessage requestResponse = await PostAsync(createEngineeringItemEndpoint, _body: engItemSetPayload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItem>>();
        }

        #endregion

        public async Task<EngineeringItemCommon> GetEngineeringItemCommon(string _engineeringItemId, EngineeringItemFields _engItemFields = null, bool _multivaluated = false)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmveng:EngItemMask.Common");
            if (_engItemFields != null)
            {
                queryParams.Add("$fields", _engItemFields.ToString());
            }
            if (_multivaluated)
            {
                queryParams.Add("$mva", "true");
            }

            return await _GetEngineeringItem<EngineeringItemCommon>(_engineeringItemId, queryParams);
        }

        public async Task<EngineeringItem> GetEngineeringItemDetails(EngineeringItem _item, EngineeringItemFields _engItemFields = null, bool _multivaluated = false)
        {
            return await GetEngineeringItemDetails(_item.id, _engItemFields, _multivaluated);
        }

        public async Task<EngineeringItem> GetEngineeringItemDetails(string _engineeringItemId, EngineeringItemFields _engItemFields = null, bool _multivaluated = false)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmveng:EngItemMask.Details");
            if (_engItemFields != null)
            {
                queryParams.Add("$fields", _engItemFields.ToString());
            }
            if (_multivaluated)
            {
                queryParams.Add("$mva", "true");
            }

            return await _GetEngineeringItem<EngineeringItem>(_engineeringItemId, queryParams);
        }

      private async Task<T> _GetEngineeringItem<T>(string _engineeringItemId, Dictionary<string, string> _queryParams) where T : Item
      {
         string searchResource = string.Format("{0}/{1}", GetBaseResource(), _engineeringItemId);

         HttpResponseMessage requestResponse = await GetAsync(searchResource, _queryParams);

         if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
         {
            //handle according to established exception policy
            throw (new EngineeringResponseException(requestResponse));
         }

         NlsLabeledItemSet<T> engItemSet =
                  await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<T>>();

         if ((engItemSet != null) && (engItemSet.totalItems == 1))
         {
            return (T)engItemSet.member[0];
         }

         return null;
      }

      #region Bulk Fetch
      public async Task<(IList<EngineeringItem>, IList<string>)> BulkFetchAsDefaultMask(string[] _engineeringItemIds, EngineeringItemFields _engItemFields = null)
      {
         Dictionary<string, string> queryParams = new Dictionary<string, string>();
         queryParams.Add("$mask", "dskern:Mask.Default");
         if (_engItemFields != null)
         {
            queryParams.Add("$fields", _engItemFields.ToString());
         }

         return await BulkFetch<EngineeringItem>(_engineeringItemIds, queryParams);
      }

      public async Task<(IList<EngineeringItemCommon>, IList<string>)> BulkFetchAsCommonMask(string[] _engineeringItemIds, EngineeringItemFields _engItemFields = null)
      {
         Dictionary<string, string> queryParams = new Dictionary<string, string>();
         queryParams.Add("$mask", "dsmveng:EngItemMask.Common");
         if (_engItemFields != null)
         {
            queryParams.Add("$fields", _engItemFields.ToString());
         }

         return await BulkFetch<EngineeringItemCommon>(_engineeringItemIds, queryParams);
      }

      public async Task<(IList<EngineeringItem>, IList<string>)> BulkFetchAsDetailMask(string[] _engineeringItemIds, EngineeringItemFields _engItemFields = null)
      {
         Dictionary<string, string> queryParams = new Dictionary<string, string>();
         queryParams.Add("$mask", "dsmveng:EngItemMask.Details");
         if (_engItemFields != null)
         {
            queryParams.Add("$fields", _engItemFields.ToString());
         }

         return await BulkFetch<EngineeringItem>(_engineeringItemIds, queryParams);
      }

      private async Task<(IList<T>, IList<string>)> BulkFetch<T>(string[] _engineeringItemIds, Dictionary<string, string> _queryParams = null) where T : Item
      {
         string bulkFetchResource = string.Format("{0}/bulkfetch", GetBaseResource());

         string payload = JsonSerializer.Serialize(_engineeringItemIds);

         HttpResponseMessage requestResponse = await PostAsync(bulkFetchResource, _queryParams, null, payload);

         if (!requestResponse.IsSuccessStatusCode)
         {
            //handle according to established exception policy
            throw (new BulkFetchException(requestResponse));
         }

         NlsLabeledBulkItemSet<T> responseContent = await requestResponse.Content.ReadFromJsonAsync<NlsLabeledBulkItemSet<T>>();

         return (responseContent.member, responseContent.nonmembers);
      }
      #endregion

      public async Task<EngineeringItem> PatchEngineeringItemAttributes(string _id, EngineeringItemPatchAttributes _atts, bool _details = true, bool _multivaluated = false)
        {
            string patchEngineeringItemEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = null;
            if (_details)
            {
                queryParams = new Dictionary<string, string> { { "$mask", "dsmveng:EngItemMask.Details" } };
            }
            if (_multivaluated)
            {
                if (queryParams == null) { queryParams = new Dictionary<string, string>(); }
                queryParams.Add("$mva", "true");
            }

            string bodyPatchMessage = JsonSerializer.Serialize(_atts);//.toJson();

            HttpResponseMessage requestResponse = await PatchAsync(patchEngineeringItemEndpoint, queryParams, null, bodyPatchMessage);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            NlsLabeledItemSet<EngineeringItem> returnSet = await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItem>>();
            if ((returnSet != null) && (returnSet.totalItems == 1))
            {
                return returnSet.member[0];
            }

            return null;
        }

        public async Task<EngineeringItem> UpdateEngineeringItem(EngineeringItem _item, bool _details = false, bool _multivaluated = false)
        {
            string patchEngineeringItemEndpoint = string.Format("{0}/{1}", GetBaseResource(), _item.id);

            EngineeringItemPatch payload = new EngineeringItemPatch(_item);

            // If EnterpriseReference or EnterpriseAttributes has no content (ex. the EIN does not exists),
            // the inner collection must be removed from paylod to avoid Bad Request response;
            // set EnterpriseReference or EnterpriseAttributes to null to skip it
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string messageBody = JsonSerializer.Serialize(payload, options);

            Dictionary<string, string> queryParams = null;
            if (_details)
            {
               queryParams = new Dictionary<string, string> { { "$mask", "dsmveng:EngItemMask.Details" } };
            }
            if (_multivaluated)
            {
                if (queryParams == null) { queryParams = new Dictionary<string, string>(); }
                queryParams.Add("$mva", "true");
            }

            HttpResponseMessage requestResponse = await PatchAsync(patchEngineeringItemEndpoint, queryParams, null, messageBody);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            NlsLabeledItemSet<EngineeringItem> returnSet = await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItem>>();
            if ((returnSet != null) && (returnSet.totalItems == 1))
            {
                return returnSet.member[0];
            }

            return null;
        }

        #region EnterpriseReference
        public async Task<EnterpriseReference> SetEnterpriseReference(EngineeringItem _item, EnterpriseReferenceCreate _itemNumber)
        {
            return await SetEnterpriseReference(_item.id, _itemNumber);
        }

        public async Task<EnterpriseReference> SetEnterpriseReference(string _itemId, EnterpriseReferenceCreate _itemNumber)
        {
            string setEnterpriseRefEndpoint = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, ENTERPRISE_REFERENCE);

            string payload = JsonSerializer.Serialize(_itemNumber);

            HttpResponseMessage requestResponse = await PostAsync(setEnterpriseRefEndpoint, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new SetEnterpriseReferenceException(requestResponse));
            }

            EnterpriseReferenceSet enterpriseRefSet = await requestResponse.Content.ReadFromJsonAsync<EnterpriseReferenceSet>();
            if ((enterpriseRefSet != null) && (enterpriseRefSet.totalItems == 1))
            {
                return enterpriseRefSet.member[0];
            }

            return null;

        }
        
        public async Task<EnterpriseReference> GetEnterpriseReference(EngineeringItem _item)
        {
            return await GetEnterpriseReference(_item.id);
        }

        public async Task<EnterpriseReference> GetEnterpriseReference(string _itemId)
        {
            string getEnterpriseRefEndpoint = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, ENTERPRISE_REFERENCE);

            HttpResponseMessage requestResponse = await GetAsync(getEnterpriseRefEndpoint);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetEnterpriseReferenceException(requestResponse));
            }

            EnterpriseReferenceSet enterpriseRefSet =
                await requestResponse.Content.ReadFromJsonAsync<EnterpriseReferenceSet>();

            if ((enterpriseRefSet != null) && (enterpriseRefSet.totalItems == 1))
            {
                return enterpriseRefSet.member[0];
            }

            return null;
        }

        //Modifies the Enterprise Reference of an Engineering item
        public async Task<EnterpriseReference> UpdateEnterpriseReference(EngineeringItem _item, EnterpriseReferenceCreate _newRef)
        {
            return await UpdateEnterpriseReference(_item.id, _newRef);
        }

        public async Task<EnterpriseReference> UpdateEnterpriseReference(string _engItemId, EnterpriseReferenceCreate _newRef)
        {
            string setEnterpriseRefEndpoint = string.Format("{0}/{1}{2}", GetBaseResource(), _engItemId, ENTERPRISE_REFERENCE);

            string messageBody = JsonSerializer.Serialize(_newRef);

            HttpResponseMessage requestResponse = await PatchAsync(setEnterpriseRefEndpoint, null, null, messageBody);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new UpdateEnterpriseReferenceException(requestResponse));
            }

            EnterpriseReferenceSet enterpriseRefSet =
                await requestResponse.Content.ReadFromJsonAsync<EnterpriseReferenceSet>();

            if ((enterpriseRefSet != null) && (enterpriseRefSet.totalItems == 1))
            {
                return enterpriseRefSet.member[0];
            }

            return null;
        }
        #endregion

        #region  dseng:EngInstance        
        //Gets all the Engineering Item Instances
        public async Task<NlsLabeledItemSet<EngineeringInstanceReference>> GetEngineeringInstances(EngineeringItem _item)
        {
            return await GetEngineeringInstances(_item.id);
        }

        public async Task<NlsLabeledItemSet<EngineeringInstanceReference>> GetEngineeringInstances(string _itemId)
        {
            string getEngineeringInstances = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, ENGINEERING_INSTANCES);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmveng:EngInstanceMask.Details");

            HttpResponseMessage requestResponse = await GetAsync(getEngineeringInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringInstanceReference>>();
        }

        public async Task< NlsLabeledItemSet<EngineeringInstanceEffectivity>> GetEngineeringInstancesEffectivity(string _itemId)
        {
            string getEngineeringInstances = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, ENGINEERING_INSTANCES);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmveng:EngInstanceMask.Filterable");

            HttpResponseMessage requestResponse = await GetAsync(getEngineeringInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringInstanceEffectivity>>();            
        }

        public async Task<NlsLabeledItemSet<EngineeringInstanceEffectivity>> GetEngineeringInstancesEffectivity(EngineeringItem _item)
        {
            return await GetEngineeringInstancesEffectivity(_item.id);
        }
        #endregion

        #region  dscfg:Filterable
        public async Task<ItemSet<EngineeringInstanceEffectivityContent>> GetEngineeringInstanceEffectivity(string _itemId, string _instanceId)
        {
            string getEngineeringInstances = string.Format("{0}/{1}{2}/{3}", GetBaseResource(), _itemId, ENGINEERING_INSTANCES, _instanceId, FILTERABLE);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvcfg:FilterableDetails");
            queryParams.Add("$fields", "dsmvcfg:attribute.effectivityContent");

            HttpResponseMessage requestResponse = await GetAsync(getEngineeringInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemSet<EngineeringInstanceEffectivityContent>>();            
        }

        public async Task< ItemSet<EngineeringInstanceEffectivityHasEffectivity>> GetEngineeringInstanceHasEffectivity(string _itemId, string _instanceId)
        {
            string getEngineeringInstances = string.Format("{0}/{1}{2}/{3}", GetBaseResource(), _itemId, ENGINEERING_INSTANCES, _instanceId, FILTERABLE);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvcfg:FilterableDetails");
            queryParams.Add("$fields", "dsmvcfg:attribute.hasEffectivity");

            HttpResponseMessage requestResponse = await GetAsync(getEngineeringInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemSet<EngineeringInstanceEffectivityHasEffectivity>>();
        }

        public async Task<ItemSet<EngineeringInstanceEffectivityHasChange>> GetEngineeringInstanceHasChangeOrder(string _itemId, string _instanceId)
        {
            string getEngineeringInstances = string.Format("{0}/{1}{2}/{3}", GetBaseResource(), _itemId, ENGINEERING_INSTANCES, _instanceId, FILTERABLE);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvcfg:FilterableDetails");
            queryParams.Add("$fields", "dsmvcfg:attribute.hasChange");

            HttpResponseMessage requestResponse = await GetAsync(getEngineeringInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemSet<EngineeringInstanceEffectivityHasChange>>();            
        }

      public async Task<NlsLabeledItemSet<EngineeringItem>> CreateEngineeringInstances(string _engItemId, EngineeringInstancesCreate _instances)
      {
         string createEngineeringInstancesEndpoint = string.Format($"{GetBaseResource()}/{_engItemId}{ENGINEERING_INSTANCES}");

         //EngineeringSearchMask engItemMask = EngineeringSearchMask.Default;

         // masks
         Dictionary<string, string> queryParams = new Dictionary<string, string>();
         queryParams.Add("$mask", "dskern: Mask.Default");

         string engInstanceSetPayload = JsonSerializer.Serialize(_instances);
         HttpResponseMessage requestResponse = await PostAsync(createEngineeringInstancesEndpoint, _body: engInstanceSetPayload);

         if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
         {
            //handle according to established exception policy
            throw (new CreateEngineeringInstancesException(requestResponse));
         }

         return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItem>>();

      }

      #endregion

      #region  dscfg:Configured
      //This extension gets the Enabled Criteria and Configuration Contexts of Configured object
      public async Task<EngineeringItemConfigurationDetails> GetConfigurationDetails(string _itemId)
        {
            string getConfigurationDetails = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, CONFIGURED);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvcfg:ConfiguredDetails");

            HttpResponseMessage requestResponse = await GetAsync(getConfigurationDetails, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            NlsLabeledItemSet<EngineeringItemConfigurationDetails> configurationSet =
                await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<EngineeringItemConfigurationDetails>>();
            
            if ((configurationSet != null) && (configurationSet.totalItems == 1))
            {
                return configurationSet.member[0];
            }

            return null;
        }

        public async Task<bool?> GetIsConfigured(string _itemId)
        {
            //string getIsConfigured = string.Format("{0}/{1}{2}", GetBaseResource(), _itemId, CONFIGURED); // Method 1 
            string getIsConfigured = string.Format("{0}/{1}", GetBaseResource(), _itemId); // Method 2

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            //queryParams.Add("$fields", "dsmvcfg:attribute.isConfigured"); // Method 1 
             queryParams.Add("$mask", "dskern:Mask.Default"); // Method 2
             queryParams.Add("$fields", "dsmvcfg:attribute.isConfigured"); // Method 2

            HttpResponseMessage requestResponse = await GetAsync(getIsConfigured, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new EngineeringResponseException(requestResponse));
            }

            ItemSet<EngineeringItemIsConfigured> isConfiguredSet =
                await requestResponse.Content.ReadFromJsonAsync<ItemSet<EngineeringItemIsConfigured>>();
            
            if ((isConfiguredSet != null) && (isConfiguredSet.totalItems == 1))
            {
                return isConfiguredSet.member[0].isConfigured;
            }

            return null;
        }

        #endregion
    }
}
