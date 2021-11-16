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
using ds.enovia.common.model;
using ds.enovia.dscfg.exception;
using ds.enovia.dscfg.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ds.enovia.dscfg.service
{
    public class ConfigurationService : EnoviaBaseService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dscfg/invoke";
        private const string UNSET_VARIANT = "/dscfg:unsetVariant";
        private const string UNSET_EVOLUTION = "/dscfg:unsetEvolution";
        private const string GET_EFFECTIVITY_CONTENT = "/dscfg:getEffectivityContent";
        private const string SET_VARIANT = "/dscfg:setVariant";
        private const string SET_EVOLUTION = "/dscfg:setEvolution";

        private const string CHANGE_ACTION_HEADER = "DS-Change-Authoring-Context";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public ConfigurationService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        // Service to unset the variant effectivities for a list of relationships. If unsetVariant service 
        // is executed under Work Under(Change Action) then it may lead to a new evolution of existing relationship.
        public async Task<ResourceEditSet> UnsetVariant(BusinessObjectIdentifier _resource, string _changeActionId = null)
        {
            string unsetVariantEndpoint = string.Format("{0}{1}", GetBaseResource(), UNSET_VARIANT);

            //Change Action header . if existing
            Dictionary<string, string> headers = null;

            if (_changeActionId != null)
            {
                headers = new Dictionary<string, string>();
                headers.Add(CHANGE_ACTION_HEADER, _changeActionId);
            }

            string payload = _resource.toJson();

            HttpResponseMessage requestResponse = await PostAsync(unsetVariantEndpoint, null, headers, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ConfigurationResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ResourceEditSet>();
        }

        // Service to unset the evolution effectivities for a list of relationships.
        public async Task<ResourceEditSet> UnsetEvolution(BusinessObjectIdentifier _resource)
        {
            string unsetEvolutionEndpoint = string.Format("{0}{1}", GetBaseResource(), UNSET_EVOLUTION);

            string payload = _resource.toJson();

            HttpResponseMessage requestResponse = await PostAsync(unsetEvolutionEndpoint, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ConfigurationResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ResourceEditSet>();
        }

        // Service to get the effectivity expression for a list of relationships
        public async Task<EffectivityContentSet> GetEffectivityContent(EffectivityContentRequest _effectivity)
        {
            string getEffectivityContentEndpoint = string.Format("{0}{1}", GetBaseResource(), GET_EFFECTIVITY_CONTENT);

            string payload = _effectivity.toJson();

            HttpResponseMessage requestResponse = await PostAsync(getEffectivityContentEndpoint, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ConfigurationResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<EffectivityContentSet>();
        }

        // Service to set the effectivities variant expression(XML) for a list of relationships. 
        // If setVariant service is executed under Work Under(Change Action) then it may lead to 
        // a new evolution of existing relationship.
        // WARNING: Coherency between Evolution and Variant Expression are under users responsibility.         
        public async Task<ResourceEditSet> SetVariant(VariantEffectivity _effectivity, string _changeActionId = null)
        {
            string setVariantEndpoint = string.Format("{0}{1}", GetBaseResource(), SET_VARIANT);

            //Change Action header . if existing
            Dictionary<string, string> headers = null;

            if (_changeActionId != null)
            {
                headers = new Dictionary<string, string>();
                headers.Add(CHANGE_ACTION_HEADER, _changeActionId);
            }

            string payload = _effectivity.toJson();

            HttpResponseMessage requestResponse = await PostAsync(setVariantEndpoint, null, headers, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ConfigurationResponseException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ResourceEditSet>();
        }

        // Service to set the effectivities evolution expression(XML) for a list of relationships.
        // WARNING: Coherency between Evolution and Variant Expression are under users responsibility.
        public async Task<ResourceEditSet> SetEvolution(EvolutionEffectivity _effectivity)
        {
            string setEvolutionEndpoint = string.Format("{0}{1}", GetBaseResource(), SET_EVOLUTION);

            string payload = _effectivity.toJson();

            HttpResponseMessage requestResponse = await PostAsync(setEvolutionEndpoint, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new ConfigurationResponseException(requestResponse));
            }
            return await requestResponse.Content.ReadFromJsonAsync<ResourceEditSet>();
        }
    }
}
