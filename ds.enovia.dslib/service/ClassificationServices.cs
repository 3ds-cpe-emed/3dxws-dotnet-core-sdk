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
using ds.enovia.dslib.exception;
using ds.enovia.dslib.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ds.enovia.dslib.service
{
    public class ClassificationServices : EnoviaBaseService
    {
        //example: "/resources/v1/modeler/dseng/dseng:EngItem/search?tenant={{tenant}}&$searchStr={{search-string}}&$mask=dsmveng:EngItemMask.Details&$top=1000&$skip=0"
        private const string BASE_RESOURCE            = "/resources/v1/modeler/dslib";
        private const string CLASSIFIED_ITEM_RESOURCE = "/dslib:ClassifiedItem";
        private const string CLASSIFIED_ITEM_LOCATE   = "/dslib:ClassifiedItem/locate";

        public ClassificationServices(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {
        }

        //Gets either the 'reverse classification path (dslib:ReverseClassificationMask)' of the given Classified Item object.
        public async Task<NlsLabeledItemSet<GeneralClassNode>> GetParentClassificationHierarchy(string physicalIdEngineeringItem)
        {
            string classifiedItemResource = string.Format("{0}{1}/{2}", BASE_RESOURCE, CLASSIFIED_ITEM_RESOURCE, physicalIdEngineeringItem);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dslib:ReverseClassificationMask");

            HttpResponseMessage requestResponse = await GetAsync(classifiedItemResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                return null;
            }

            return await requestResponse.Content.ReadFromJsonAsync<NlsLabeledItemSet<GeneralClassNode>>();
        }

        //Gets the 'classification attributes (dslib:ClassificationAttributesMask)' of the given Classified Item object.
        public async Task<ItemsClassificationAttributes> GetClassificationAttributes(ClassifiedItemInstance classifiedItemInstance)
        {
            string classifiedItemResource = string.Format("{0}{1}/{2}", BASE_RESOURCE, CLASSIFIED_ITEM_RESOURCE, classifiedItemInstance.ClassifiedItem.identifier);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add("$mask", "dslib:ClassificationAttributesMask");

            HttpResponseMessage requestResponse = await GetAsync(classifiedItemResource, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                return null;
            }

            return await requestResponse.Content.ReadFromJsonAsync<ItemsClassificationAttributes>();
        }

        //etches the object reference of the Classified Item of a given external object (public http resource) if it exists.
        public async Task<ClassifiedItemInstance> Locate(BusinessObjectIdentifier itemInstance)
        {
            string classifiedItemResource = string.Format("{0}{1}", BASE_RESOURCE, CLASSIFIED_ITEM_LOCATE);

            string payload = JsonSerializer.Serialize<object>(itemInstance);

            HttpResponseMessage requestResponse = await PostAsync(classifiedItemResource, null, null, payload);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ClassifiedItemLocationResponseException(requestResponse);
            }

            return await requestResponse.Content.ReadFromJsonAsync<ClassifiedItemInstance>();
        }
    }
}
