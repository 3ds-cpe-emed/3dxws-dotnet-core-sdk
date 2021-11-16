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
using ds.enovia.common.search;
using ds.enovia.dsxcad.exception;
using ds.enovia.dsxcad.model;
using ds.enovia.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ds.enovia.dsxcad.service
{
    public enum xCADDrawingDetails
    {
        Basic,
        Details,
        EnterpriseDetails
    }

    public class xCADDrawingService : xCADDesignIntegrationService
    {        
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsxcad/dsxcad:Drawing";
        private const string SEARCH = "/search";

        public override string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public xCADDrawingService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }


        public async Task<xCADDrawing> GetXCADDrawing(string _id, xCADDrawingDetails _details = xCADDrawingDetails.Basic)
        {
            string getXCADDrawingEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (_details != default(xCADDrawingDetails))
            {
                queryParams.Add("$mask", GetMaskString(_details));
            }

            HttpResponseMessage requestResponse = await GetAsync(getXCADDrawingEndpoint, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADDrawingException(requestResponse));
            }

            xCADDrawingSet xcadDWGSet = await requestResponse.Content.ReadFromJsonAsync<xCADDrawingSet>();
            
            if ((xcadDWGSet != null) && (xcadDWGSet.totalItems == 1))
            {
                return xcadDWGSet.member[0];
            }

            return null;
        }

        public async Task<FileInfo> DownloadAuthoringFile(HttpClient _downloadHttpClient, string _dwgId, string _downloadLocation)
        {
            FileDownloadTicket fileDownloadTicket = await GetAuthoringFileDownloadTicket(GetBaseResource(), _dwgId);

            if (fileDownloadTicket == null)
                throw new Exception($"unknown error getting download ticket for authoring file of Drawing with id='{_dwgId}'");

            return await DownloadFile(_downloadHttpClient, fileDownloadTicket, _downloadLocation);

        }

        private string GetMaskString(xCADDrawingDetails _details)
        {
            string __mask = "dsmvxcad:xCADDrawingMask.Basic";

            switch (_details)
            {
                case xCADDrawingDetails.Details:
                    __mask = "dsmvxcad:xCADDrawingMask.Details";
                    break;

                case xCADDrawingDetails.EnterpriseDetails:
                    __mask = "dsmvxcad:xCADDrawingMask.EnterpriseDetails";
                    break;
            }

            return __mask;
        }

        public async Task<IList<xCADDrawing>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, xCADDrawingDetails _mask = xCADDrawingDetails.Basic)
        {
            return await SearchAll<xCADDrawing>(_searchString, GetMaskString(_mask), _top);
        }

        //Modifies the Drawing attributes
        public async Task<xCADDrawing> PatchXCADDrawingAttributes(string _id, xCADDrawingPatchAttributes _atts, bool _details = true)
        {
            string patchXCADDrawingEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = null;
            if (_details)
            {
                queryParams = new Dictionary<string, string>();
                queryParams.Add("$mask", "dsmvxcad:xCADDrawingMask.EnterpriseDetails");
            }

            string bodyPatchMessage = _atts.toJson();
            
            HttpResponseMessage requestResponse = await PatchAsync(patchXCADDrawingEndpoint, queryParams, null, bodyPatchMessage);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADDrawingException(requestResponse));
            }

            xCADDrawingSet xcadDWGSet = await requestResponse.Content.ReadFromJsonAsync<xCADDrawingSet>();
            if ((xcadDWGSet != null) && (xcadDWGSet.totalItems == 1))
            {
                return xcadDWGSet.member[0];
            }

            return null;
        }
    }
}
