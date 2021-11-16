using ds.enovia.common.constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ds.enovia.common.helper
{
    public class GetHttpJsonMessage : HttpRequestMessage
    {
        public GetHttpJsonMessage(Uri _uri) : base(HttpMethod.Get, _uri)
        {
            this.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MimeTypes.APPLICATION_JSON));
        }
        
    }
}
