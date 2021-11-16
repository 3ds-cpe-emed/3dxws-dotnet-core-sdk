using ds.enovia.common.constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ds.enovia.common.helper
{
    public class PostHttpJsonMessage : HttpRequestMessage
    {
        public PostHttpJsonMessage(Uri _uri, string _jsonPayload = null) : base(HttpMethod.Post, _uri)
        {
            this.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MimeTypes.APPLICATION_JSON));

            if (_jsonPayload != null)
                this.Content = new StringContent(_jsonPayload, Encoding.UTF8, MimeTypes.APPLICATION_JSON);
        }
    }
}
