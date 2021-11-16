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
using ds.enovia.dslc.changeaction.exception;
using ds.enovia.dslc.changeaction.model;
using ds.enovia.service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ds.enovia.dslc.changeaction.service
{
    public class ChangeActionServices : EnoviaBaseService
    {        
        private const string BASE_RESOURCE = "/resources/v1/modeler/dslc/changeaction";
      
        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public ChangeActionServices(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }

        public async Task<ChangeAction> GetChangeAction(string _id, bool detailed = true)
        {
            string getChangeActionById = string.Format("{0}/{1}", GetBaseResource(), _id);

            Dictionary<string, string> queryParamsDictionary = null;
            if (detailed)
            {
                queryParamsDictionary = new Dictionary<string, string>();
                queryParamsDictionary.Add("$fields", "members,proposedChanges,realizedChanges,referentials,contexts");
            }

            HttpResponseMessage requestResponse = await GetAsync(getChangeActionById, queryParamsDictionary);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetChangeActionException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<ChangeAction>();
        }
    }
}
