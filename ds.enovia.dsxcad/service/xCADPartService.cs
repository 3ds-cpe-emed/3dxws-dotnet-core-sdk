//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2021 Dassault Systèmes - CPE EMED
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
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dsxcad.exception;
using ds.enovia.dsxcad.model;

namespace ds.enovia.dsxcad.service
{
    public enum xCADPartDetails
    {
        Basic,
        Details,
        EnterpriseDetails
    }

    public class xCADPartService : xCADDesignIntegrationService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsxcad/dsxcad:Part";
 

        public override string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public xCADPartService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }

     
        public async Task<xCADPart> GetXCADPart(string _id, xCADPartDetails _details = xCADPartDetails.Basic)
        {
            string getXCADPartEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (_details != default(xCADPartDetails))
            {
                queryParams.Add("$mask", GetMaskString(_details));
            }

            HttpResponseMessage requestResponse = await GetAsync(getXCADPartEndpoint, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADPartException(requestResponse));
            }

            xCADPartSet xcadPartSet = await requestResponse.Content.ReadFromJsonAsync<xCADPartSet>();
            
            if ((xcadPartSet != null) && (xcadPartSet.totalItems == 1))
            {
                return xcadPartSet.member[0];
            }

            return null;
        }

 
        public async Task DownloadAuthoringFile(HttpClient _downloadHttpClient, string _partId, string _downloadLocation)
        {
            FileDownloadTicket fileDownloadTicket = await GetAuthoringFileDownloadTicket(GetBaseResource(), _partId);
        
            if (fileDownloadTicket == null)
                throw new Exception($"unknown error getting download ticket for authoring file of Part with id='{_partId}'");

            await DownloadFile(_downloadHttpClient, fileDownloadTicket, _downloadLocation);

            return;
        }

        private string GetMaskString(xCADPartDetails _details)
        {
            string __mask = "dsmvxcad:xCADPartMask.Basic";

            switch (_details)
            {
                case xCADPartDetails.Details:
                    __mask ="dsmvxcad:xCADPartMask.Details";
                    break;

                case xCADPartDetails.EnterpriseDetails:
                    __mask = "dsmvxcad:xCADPartMask.EnterpriseDetails";
                    break;
            }

            return __mask;
        }

        public async Task<IList<xCADPart>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, xCADPartDetails _mask = xCADPartDetails.Basic)
        {
            return await SearchAll<xCADPart>(_searchString, GetMaskString(_mask), _top);
        }

        //Modifies the Drawing attributes
        //public async Task<xCADDrawing> PatchXCADDrawingAttributes(string _id, xCADDrawingPatchAttributes _atts, bool _details = true)
        //{
        //    string patchXCADDrawingEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

        //    Dictionary<string, string> queryParams = null;
        //    if (_details)
        //    {
        //        queryParams = new Dictionary<string, string>();
        //        queryParams.Add("$mask", "dsmvxcad:xCADDrawingMask.EnterpriseDetails");
        //    }

        //    string bodyPatchMessage = _atts.toJson();

        //    HttpResponseMessage requestResponse = await PatchAsync(patchXCADDrawingEndpoint, queryParams, null, bodyPatchMessage);

        //    if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        //handle according to established exception policy
        //        throw (new GetXCADDrawingException(requestResponse));
        //    }

        //    xCADDrawingSet xcadDWGSet = await requestResponse.Content.ReadFromJsonAsync<xCADDrawingSet>();
        //    if ((xcadDWGSet != null) && (xcadDWGSet.totalItems == 1))
        //    {
        //        return xcadDWGSet.member[0];
        //    }

        //    return null;
        //}
    }
}
