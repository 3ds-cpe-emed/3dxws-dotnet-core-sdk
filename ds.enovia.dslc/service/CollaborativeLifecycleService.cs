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

using ds.authentication;

using ds.enovia.dslc.exception;
using ds.enovia.dslc.model;
using ds.enovia.dslc.serialization;
using ds.enovia.service;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ds.enovia.dslc.service
{
    public class CollaborativeLifecycleService : EnoviaBaseService
    {
        private const string BASE_RESOURCE      = "/resources/v1/modeler/dslc";
        private const string VERSION_CREATE     = "/version/create";
        private const string VERSION_GRAPH      = "/version/getGraph";
        private const string TRANSFER_OWNERSHIP = "/ownership/transfer";

        public string GetBaseResource()
        {
            return BASE_RESOURCE;
        }

        public CollaborativeLifecycleService(string _enoviaService, IPassportAuthentication passport) : base(_enoviaService, passport)
        {

        }
        
        //Note : It was verified that the source property of the input should be null (R2022XGA) this might change in future versions
        public async Task<VersionGraph> GetVersionGraph(BusinessObjectData _businessObjectIds)
        {
            string getVersionGraphResourceUrl = string.Format("{0}{1}", GetBaseResource(), VERSION_GRAPH);

            string bodyVersionGraph = JsonSerializer.Serialize(_businessObjectIds);

            HttpResponseMessage requestResponse = await PostAsync(getVersionGraphResourceUrl, _body: bodyVersionGraph);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //handle according to established exception policy
                throw (new GetGraphVersionException(requestResponse));
            }

            return await requestResponse.Content.ReadFromJsonAsync<VersionGraph>();

         }

         public async Task<IOwnershipTransferResponse> TransferOwnership(IOwnershipTransferData _ownershipTransferData)
         {
            string getTransferOwnershipResourceUrl = string.Format("{0}{1}", GetBaseResource(), TRANSFER_OWNERSHIP);

            string bodyTransferOwnership = JsonSerializer.Serialize<object>(_ownershipTransferData);

            HttpResponseMessage requestResponse = await PostAsync(getTransferOwnershipResourceUrl, _body: bodyTransferOwnership);

            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
               //handle according to established exception policy
               throw (new TransferOwnershipException(requestResponse));
            }

            // Deserialize with interface implementation classes to be used during serial/deserialization defined in options through custom type converter

            JsonSerializerOptions options = new JsonSerializerOptions {
                  Converters =
                  {
                      new TypeMappingConverter<IOwnershipTransferStatus, OwnershipTransferStatus>(),
                      new TypeMappingConverter<IOwnershipTransferResponse, OwnershipTransferResponse>()
                  }
            };
         
            return await requestResponse.Content.ReadFromJsonAsync<IOwnershipTransferResponse>(options);
         }
   }
}
