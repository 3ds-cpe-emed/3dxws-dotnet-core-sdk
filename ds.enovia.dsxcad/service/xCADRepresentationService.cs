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
    public enum xCADRepresentationDetails
   {
        Default,
        Basic,
        Details
    }

    public class xCADRepresentationService : xCADDesignIntegrationService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsxcad/dsxcad:Representation";
 
        public override string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public xCADRepresentationService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }
     
        public async Task<xCADRepresentation> GetXCADRepresentation(string _id, xCADRepresentationDetails _details = xCADRepresentationDetails.Basic)
        {
            string getXCADRepresentationEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (_details != default(xCADRepresentationDetails))
            {
                queryParams.Add("$mask", GetMaskString(_details));
            }

            HttpResponseMessage requestResponse = await GetAsync(getXCADRepresentationEndpoint, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADRepresentationException(requestResponse));
            }

            xCADRepresentationSet xcadRepSet = await requestResponse.Content.ReadFromJsonAsync<xCADRepresentationSet>();
            
            if ((xcadRepSet != null) && (xcadRepSet.totalItems == 1))
            {
                return xcadRepSet.member[0];
            }

            return null;
        }

        public async Task DownloadAuthoringFile(HttpClient _downloadHttpClient, string _repId, string _downloadLocation)
        {
            FileDownloadTicket fileDownloadTicket = await GetAuthoringFileDownloadTicket(GetBaseResource(), _repId);
        
            if (fileDownloadTicket == null)
                throw new Exception($"unknown error getting download ticket for authoring file of Representation with id='{_repId}'");

            await DownloadFile(_downloadHttpClient, fileDownloadTicket, _downloadLocation);

            return;
        }

        private string GetMaskString(xCADRepresentationDetails _details)
        {
            string __mask = "dsmvxcad:xCADRepMask.Default";

            switch (_details)
            {
                case xCADRepresentationDetails.Details:
                    __mask = "dsmvxcad:xCADRepMask.Details";
                    break;

                case xCADRepresentationDetails.Basic:
                    __mask = "dsmvxcad:xCADRepMask.Basic";
                    break;
            }

            return __mask;
        }

        public async Task<IList<xCADRepresentation>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, xCADRepresentationDetails _mask = xCADRepresentationDetails.Default)
        {
            return await SearchAll<xCADRepresentation>(_searchString, GetMaskString(_mask), _top);
        }

    }
}
