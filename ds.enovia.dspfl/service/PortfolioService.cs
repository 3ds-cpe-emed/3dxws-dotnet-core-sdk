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
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.dspfl.exception;
using ds.enovia.dspfl.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ds.enovia.dspfl.service
{
    public class PortfolioService : EnoviaBaseService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dspfl";
        private const string MODEL_VERSION = "/dspfl:ModelVersion";
        private const string PRODUCT_CONFIGURATION = "/dspfl:ProductConfiguration";
        private const string VARIANT = "/dspfl:Variant";
        private const string VARIANT_INSTANCE = "/dspfl:VariantInstance";
        private const string VARIABILITY_GROUP = "/dspfl:VariabilityGroup";
        private const string OPTION_GROUP_INSTANCES = "/dspfl:OptionGroupInstance";
        private const string OPTION = "/dspfl:Option";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public PortfolioService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        #region dspfl:ModelVersion
        //Get details of all Model Versions.
        public async Task<NlsLabeledItemSet<ModelVersion>> GetAllModelVersions()
        {
            string getAllModelVersions = string.Format("{0}{1}", GetBaseResource(), MODEL_VERSION);

            // masks
            // Mask defining what will be returned. Default Mask is:dsmvpfl: ModelVersionBaseMask
            // dsmvpfl:ModelVersionBaseMask, dsmvpfl:ModelVersionDetailMask
            // from a simple test there was no visible difference between any of the two masks
            HttpResponseMessage requestResponse = await GetAsync(getAllModelVersions);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ModelVersion>>();
        }
        #endregion

        #region dspfl:Variant
        public async Task<NlsLabeledItemSet<Variant>> GetAllVariants()
        {
            string getAllModelVersions = string.Format("{0}{1}", GetBaseResource(), VARIANT);

            // masks
            // Mask defining what will be returned. Default Mask is:dsmvpfl: ModelVersionBaseMask
            // dsmvpfl:ModelVersionBaseMask, dsmvpfl:ModelVersionDetailMask
            // from a simple test there was no visible difference between any of the two masks
            HttpResponseMessage requestResponse = await GetAsync(getAllModelVersions);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<Variant>>();
        }
        #endregion

        #region dspfl:VariantInstance
        //Get all allocated Variants and Variant Values of given Model Version.
        public async Task<NlsLabeledItemSet<VariantInstance>> GetVariantInstances(string _modelVersionId)
        {
            string getVariantInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MODEL_VERSION, _modelVersionId, VARIANT_INSTANCE);

            //not well documented unfortunately
            //dsmvpfl:VariantValuesBaseMask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:VariantInstanceMask");

            HttpResponseMessage requestResponse = await GetAsync(getVariantInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<VariantInstance>>();            
        }

        #endregion

        #region dspfl:VariantValue
        public async Task<NlsLabeledItemSet<VariantValues>> GetVariantValues(string _variantId)
        {
            string getAllModelVersions = string.Format("{0}{1}/{2}", GetBaseResource(), VARIANT, _variantId);

            //not well documented unfortunately
            //dsmvpfl:VariantValuesBaseMask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:VariantValuesBaseMask");

            HttpResponseMessage requestResponse = await GetAsync(getAllModelVersions, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<VariantValues>>();
        }
        #endregion

        #region dspfl:VariabilityGroup
        //Get details of all Variability Groups.
        public async Task<NlsLabeledItemSet<VariabilityGroup>> GetAllVariabilityGroups()
        {
            string getVariabilityGroups = string.Format("{0}{1}", GetBaseResource(), VARIABILITY_GROUP);

            //not well documented unfortunately
            //dsmvpfl:VariantValuesBaseMask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:VariabilityGroupBaseMask");

            HttpResponseMessage requestResponse = await GetAsync(getVariabilityGroups, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<VariabilityGroup>>();
        }
        #endregion

        #region dspfl:OptionGroupInstance
        //Get all allocated Variability Groups and Options of given Model Version.
        public async Task<NlsLabeledItemSet<OptionGroupInstance>> GetOptionGroupInstances(string _modelVersionId)
        {
            string getOptionGroupInstances = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MODEL_VERSION,  _modelVersionId, OPTION_GROUP_INSTANCES);

            //not well documented unfortunately
            //dsmvpfl:VariantValuesBaseMask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:OptionGroupInstanceMask");

            HttpResponseMessage requestResponse = await GetAsync(getOptionGroupInstances, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<OptionGroupInstance>>();
        }

        #endregion

        #region dspfl:Option
        //Get details of all Options of given Variability Group.
        public async Task<NlsLabeledItemSet<OptionGroupInstance>> GetOptions(string _variabilityGroupId)
        {
            string getOptions = string.Format("{0}{1}/{2}{3}", GetBaseResource(), VARIABILITY_GROUP, _variabilityGroupId, OPTION);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:OptionBaseMask");

            HttpResponseMessage requestResponse = await GetAsync(getOptions, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<OptionGroupInstance>>();
        }

        #endregion

        #region dspfl:ProductConfiguration

        public async Task<NlsLabeledItemSet<ProductConfiguration>> CreateProductConfiguration(string _modelVersionId, ProductConfigurationCreate _productConfigurationCreate)
        {
            string productConfigurationUri = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MODEL_VERSION, _modelVersionId, PRODUCT_CONFIGURATION);

            //masks
            //dsmvpfl:ProductConfigurationBaseMask, dsmvpfl:ProductConfigurationCriteriaMask
            ProductConfigurationCreateItems createItems = new ProductConfigurationCreateItems();
            createItems.items.Add(_productConfigurationCreate);

            string payload = JsonSerializer.Serialize(createItems);

            HttpResponseMessage requestResponse = await PostAsync(productConfigurationUri, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ProductConfiguration>>();
        }

        // Get details of all Product Configurations of given Model Version
        public async Task<NlsLabeledItemSet<ProductConfiguration>> GetAllProductConfigurations(string _modelVersionId)
        {
            string getAllProductConfigurations = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MODEL_VERSION, _modelVersionId, PRODUCT_CONFIGURATION);

            //masks
            //dsmvpfl:ProductConfigurationBaseMask, dsmvpfl:ProductConfigurationCriteriaMask

            HttpResponseMessage requestResponse = await GetAsync(getAllProductConfigurations);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ProductConfiguration>>();
        }

        // Get details of all Product Configurations of given Model Version
        public async Task<NlsLabeledItemSet<ProductConfigurationCriteria>> GetAllProductConfigurationsWithCriteria(string _modelVersionId, bool _criteriaResolved = false)
        {
            string getAllProductConfigurations = string.Format("{0}{1}/{2}{3}", GetBaseResource(), MODEL_VERSION, _modelVersionId, PRODUCT_CONFIGURATION);

            //masks
            //dsmvpfl:ProductConfigurationBaseMask, dsmvpfl:ProductConfigurationCriteriaMask
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dsmvpfl:ProductConfigurationCriteriaMask");

            HttpResponseMessage requestResponse = await GetAsync(getAllProductConfigurations, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new PortfolioResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<ProductConfigurationCriteria>>();
        }
        #endregion
    }
}
