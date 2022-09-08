//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2022 Dassault Systèmes - CPE EMED
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
using System.Threading.Tasks;

using ds.authentication;

using ds.enovia.common.search;
using ds.enovia.dsxcad.exception;
using ds.enovia.dsxcad.model;

namespace ds.enovia.dsxcad.service
{
    public enum xCADProductDetails
   {
        Basic,
        Details,
        EnterpriseDetails
    }

    public class xCADProductService : xCADDesignIntegrationService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsxcad/dsxcad:Product";
 
        public override string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public xCADProductService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }
     
        public async Task<xCADProduct> GetXCADProduct(string _id, xCADProductDetails _details = xCADProductDetails.Basic)
        {
            string getXCADProductEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (_details != default(xCADProductDetails))
            {
                queryParams.Add("$mask", GetMaskString(_details));
            }

            HttpResponseMessage requestResponse = await GetAsync(getXCADProductEndpoint, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADProductException(requestResponse));
            }

            xCADProductSet xcadProductSet = await requestResponse.Content.ReadFromJsonAsync<xCADProductSet>();
            
            if ((xcadProductSet != null) && (xcadProductSet.totalItems == 1))
            {
                return xcadProductSet.member[0];
            }

            return null;
        }

        public async Task DownloadAuthoringFile(HttpClient _downloadHttpClient, string _productId, string _downloadLocation)
        {
            FileDownloadTicket fileDownloadTicket = await GetAuthoringFileDownloadTicket(GetBaseResource(), _productId);
        
            if (fileDownloadTicket == null)
                throw new Exception($"unknown error getting download ticket for authoring file of Product with id='{_productId}'");

            await DownloadFile(_downloadHttpClient, fileDownloadTicket, _downloadLocation);

            return;
        }

        private string GetMaskString(xCADProductDetails _details)
        {
            string __mask = "dsmvxcad:xCADProductMask.Default";

            switch (_details)
            {
                case xCADProductDetails.Details:
                    __mask = "dsmvxcad:xCADProductMask.Details";
                    break;

                case xCADProductDetails.EnterpriseDetails:
                    __mask = "dsmvxcad:xCADProductMask.EnterpriseDetails";
                    break;
            }

            return __mask;
        }

        public async Task<IList<xCADProduct>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, xCADProductDetails _mask = xCADProductDetails.Basic)
        {
            return await SearchAll<xCADProduct>(_searchString, GetMaskString(_mask), _top);
        }

    }
}
