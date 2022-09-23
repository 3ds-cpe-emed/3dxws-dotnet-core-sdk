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
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using ds.authentication;
using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dsxcad.exception;
using ds.enovia.dsxcad.model;

namespace ds.enovia.dsxcad.service
{
    public enum xCADFamilyRepresentationDetails
    {
        Basic,
        Details
    }

    public class xCADFamilyRepresentationService : xCADDesignIntegrationService
    {
        private const string BASE_RESOURCE = "/resources/v1/modeler/dsxcad/dsxcad:FamilyRepresentation";
        private const string LOCATE = "/locate";

        public override string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public xCADFamilyRepresentationService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }
     
        public async Task<xCADFamilyRepresentation> GetXCADFamilyRepresentation(string _id, xCADFamilyRepresentationDetails _details = xCADFamilyRepresentationDetails.Basic)
        {
            string getXCADPartEndpoint = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (_details != default(xCADFamilyRepresentationDetails))
            {
                queryParams.Add("$mask", GetMaskString(_details));
            }

            HttpResponseMessage requestResponse = await GetAsync(getXCADPartEndpoint, queryParams);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetXCADPartException(requestResponse));
            }

            xCADFamilyRepresentationSet xcadPartSet = await requestResponse.Content.ReadFromJsonAsync<xCADFamilyRepresentationSet>();
            
            if ((xcadPartSet != null) && (xcadPartSet.totalItems == 1))
            {
                return xcadPartSet.member[0];
            }

            return null;
        }

        public async Task<FileInfo> DownloadAuthoringFile(HttpClient _downloadHttpClient, string _familyRepId, string _downloadLocation)
        {
            FileDownloadTicket fileDownloadTicket = await GetAuthoringFileDownloadTicket(GetBaseResource(), _familyRepId);
        
            if (fileDownloadTicket == null)
                throw new Exception($"unknown error getting download ticket for authoring file of XCAD Family Representation with id='{_familyRepId}'");

            return await DownloadFile(_downloadHttpClient, fileDownloadTicket, _downloadLocation);
        }

        private string GetMaskString(xCADFamilyRepresentationDetails _details)
        {
            string __mask = "dsmvxcad:xCADFamilyRepMask.Basic";

            switch (_details)
            {
                case xCADFamilyRepresentationDetails.Details:
                    __mask = "dsmvxcad:xCADFamilyRepMask.Details";
                    break;
            }

            return __mask;
        }

        public async Task<IList<xCADFamilyRepresentation>> Search(SearchQuery _searchString, long _skip = 0, long _top = 100, xCADFamilyRepresentationDetails _mask = xCADFamilyRepresentationDetails.Basic)
        {
            return await SearchAll<xCADFamilyRepresentation>(_searchString, GetMaskString(_mask), _top);
        }

      // This accepts multiple inputs in one request however this doesn't seem very
      // stable in the current implementation. If I pass in the payload an number of ids that are members of
      // a CAD Family all seems ok (even if I have some doubts on the returning order in order to match the input).
      // However, if I pass an id (a valid id) then I get an error back (400 - Something happened during Dependency Link completion.)
      // if there is only one in the payload input array I get that and I am not able to tell to which element this
      // problem is related. So, for security I am restricting this to only one id.
      public async Task<MemberSet<xCADFamilyRepresentationReference>> Locate(BusinessObjectId _id)
      {
         BusinessObjectIdentifier businessObjectIdentifier = new BusinessObjectIdentifier();
         businessObjectIdentifier.identifier   = _id.id;
         businessObjectIdentifier.source       = _id.source;
         businessObjectIdentifier.relativePath = _id.relativePath;
         businessObjectIdentifier.type         = _id.type;
         return await Locate(businessObjectIdentifier);
      }

      public async Task<MemberSet<xCADFamilyRepresentationReference>> Locate(BusinessObjectIdentifier _id)
      {
         // - prepare data ---
         IList<BusinessObjectIdentifier> inputColl = new List<BusinessObjectIdentifier>();

         inputColl.Add(_id);

         string inputMessage = JsonSerializer.Serialize(inputColl);

         // - call web service ---

         string locate = string.Format("{0}{1}", GetBaseResource(), LOCATE);

         HttpResponseMessage requestResponse = await PostAsync(locate, null, null, inputMessage);

         // - handle response ---
         if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
         {
            //handle according to established exception policy
            throw (new LocateXCADFamilyException(requestResponse));
         }

         return await requestResponse.Content.ReadFromJsonAsync<MemberSet<xCADFamilyRepresentationReference>>();

      }
   }
}
