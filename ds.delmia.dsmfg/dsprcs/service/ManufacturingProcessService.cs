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
using ds.delmia.dsprcs.mask;
using ds.delmia.dsmfg.model;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.service;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ds.delmia.dsprcs.model;
using ds.delmia.model;

namespace ds.delmia.dsprcs.service
{
    public class ManufacturingProcessService : EnoviaBaseService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsprcs";
        private const string SEARCH = "/search";
        private const string MANUFACTURING_PROCESS = "/dsprcs:MfgProcess";
        private const string MANUFACTURING_PROCESS_INSTANCE = "/dsprcs:MfgProcessInstance";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public ManufacturingProcessService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        #region dsmfg:ManufacturingProcess

        //Gets a indexed search result of Manufacturing Item
        public async Task<NlsLabeledItemSet<ManufacturingProcess>> Search(SearchQuery _searchQuery, long _skip = 0, long _top = 100)
        {
            string searchManufacturingItems = string.Format("{0}{1}{2}", GetBaseResource(), MANUFACTURING_PROCESS, SEARCH);

            ManufacturingProcessMask mfgProcessMask = ManufacturingProcessMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgProcessMask.GetString());
            queryParams.Add("$skip", _skip.ToString());
            queryParams.Add("$top", _top.ToString());
            queryParams.Add("$searchStr", _searchQuery.GetSearchString());

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingProcess>>();
        }

        public async Task<NlsLabeledItemSet<ManufacturingProcess>> CreateManufacturingProcess(ManufacturingProcessCreate _mfgProcess)
        {
            ManufacturingModelSetCreate itemsCreate = new ManufacturingModelSetCreate();
            itemsCreate.items.Add(_mfgProcess);

            return await CreateManufacturingProcess(itemsCreate);
        }

        public async Task<NlsLabeledItemSet<ManufacturingProcess>> CreateManufacturingProcess(ManufacturingModelSetCreate _mfgProcessSet)
        {
            //.................

            string createManufacturingProcessEndpoint = string.Format("{0}{1}", GetBaseResource(), MANUFACTURING_PROCESS);

            ManufacturingProcessMask mfgProcessMask = ManufacturingProcessMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgProcessMask.GetString());

            string mfgProcessSetPayload = JsonSerializer.Serialize(_mfgProcessSet);

            HttpResponseMessage requestResponse = await PostAsync(createManufacturingProcessEndpoint, null, null, mfgProcessSetPayload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            //TODO: Review this
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingProcess>>(deserializeOptions);
        }

        public async Task<NlsLabeledItemSet<ManufacturingProcess>> GetManufacturingProcess(string _mfgProcessId)
        {
            string searchManufacturingProcesses = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_PROCESS, _mfgProcessId);

            ManufacturingProcessMask mfgProcessMask = ManufacturingProcessMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgProcessMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(searchManufacturingProcesses, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingProcess>>(deserializeOptions);
        }

        //TODO: Review
        //public async Task<NlsLabeledItemSet<ManufacturingItem>> GetdManufacturingItemFields(string _mfgItemId, ManufacturingItemFields _mfgItemFields)
        //{
        //    string searchManufacturingItems = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

        //    ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Default;

        //    // masks
        //    Dictionary<string, string> queryParams = new Dictionary<string, string>();
        //    queryParams.Add("$mask", mfgItemMask.GetString());
        //    queryParams.Add("$fields", _mfgItemFields.ToString());

        //    HttpResponseMessage requestResponse = await GetAsync(searchManufacturingItems, queryParams);

        //    if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        //handle according to established exception policy
        //        throw (new ManufacturingResponseException(requestResponse));
        //    }

        //    var deserializeOptions = new JsonSerializerOptions();
        //    deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

        //    return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItem>>(deserializeOptions);
        //}

        //public async Task<NlsLabeledItemSet<ManufacturingItemDetails>> GetManufacturingItemFieldsDetails(string _mfgItemId, ManufacturingItemFields _fields)
        //{
        //    string getManufacturingItem = string.Format("{0}{1}/{2}", GetBaseResource(), MANUFACTURING_ITEM, _mfgItemId);

        //    ManufacturingItemMask mfgItemMask = ManufacturingItemMask.Details;

        //    // masks
        //    Dictionary<string, string> queryParams = new Dictionary<string, string>();
        //    queryParams.Add("$mask", mfgItemMask.GetString());
        //    queryParams.Add("$fields", _fields.ToString());

        //    HttpResponseMessage requestResponse = await GetAsync(getManufacturingItem, queryParams);

        //    if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        //handle according to established exception policy
        //        throw (new ManufacturingResponseException(requestResponse));
        //    }

        //    var deserializeOptions = new JsonSerializerOptions();
        //    deserializeOptions.Converters.Add(new ManufacturingItemDetailsConverter());

        //    return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingItemDetails>>(deserializeOptions);

        //}

        #endregion

        #region dsmfg:ManufacturingInstance

        // Gets all the Manufacturing Item Instances
        public async Task<NlsLabeledItemSet<ManufacturingProcessInstance>> GetManufacturingProcessInstances(string _mfgProcessId)
        {
            string getManufacturingProcessInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_PROCESS, _mfgProcessId, MANUFACTURING_PROCESS_INSTANCE);

            ManufacturingProcessInstanceMask mfgProcessInstanceMask = ManufacturingProcessInstanceMask.Default;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgProcessInstanceMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingProcessInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingProcessInstance>>();
        }

        public async Task<NlsLabeledItemSet<ManufacturingProcessInstanceRef>> GetManufacturingProcessInstancesWithReference(string _mfgProcessId)
        {
            string getManufacturingProcessInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MANUFACTURING_PROCESS, _mfgProcessId, MANUFACTURING_PROCESS_INSTANCE);

            ManufacturingProcessInstanceMask mfgProcessInstanceMask = ManufacturingProcessInstanceMask.Details;

            // masks
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", mfgProcessInstanceMask.GetString());

            HttpResponseMessage requestResponse = await GetAsync(getManufacturingProcessInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ManufacturingResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ManufacturingProcessInstanceRef>>();

        }
        #endregion
    }
}
