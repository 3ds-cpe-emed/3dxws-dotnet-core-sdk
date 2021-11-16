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
//----

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.enovia.eif
{
    //Taken from https://media.3ds.com/support/documentation/developer/Cloud/en/English/CAAiamRESTCommon/CAARwsEventTaEventFormatPub.htm
    [JsonConverter(typeof(MessageConverter))]
    public class Message
    {
        //The web semantic versioning of the message format.
        public string specversion { get; set; }
        
        //The message identifier
        public string id { get; set; }

        //The date and time when the event happened. Its format is usually for human being.
        public string time { get; set; }

        //The date and time when the event happened in a more convenient format for automatic processing.
        public long timestamp { get; set; }

        //The type event name
        public string type { get; set; }

        // This describes the event producer. Often this will include information such as the type of the event source, the organization publishing the event, the process that produced the event, and some unique identifiers.
        // The exact syntax and semantics behind the data encoded in the URI is event producer defined.
        public string source { get; set; }

        // Applicative information.
        // The JSON is only one depth level. In other words A key value cannot be a JSON itse
        public IDictionary<string,object> metadata { get; set; }

        //The MIME type of the applicative data.
        public string contenttype { get; set; }

        // An applicative versioning. It could be the URL of the page where the applicative data contents is described.still under study.
        //public string contentschemaurl { get; set; }

        // The message data.Its contents depends on type and the contentschemaurl keys.It is encoded into a media format which is specified by the contenttype value(e.g.application/json).
        // The Event Message Data Format article gives the data structure format for the current exposed event types
        public object data { get; set; }

    }
}
