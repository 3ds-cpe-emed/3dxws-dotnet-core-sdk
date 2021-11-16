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


using System.Text.Json.Serialization;

namespace ds.enovia.eif
{
    public class EventSubject
    {
        //<summary>Source of the object. As explained earlier exact syntax is producer defined.</summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        //<summary>The type of the current/related object.</summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        //<summary>The object identifier.as specified by the service exposing its resources.</summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        //<summary>Endpoint to read object metadata</summary>
        [JsonPropertyName("relativePath")]
        public string RelativePath { get; set; }
    
    }
    public class EventObject
    {
        //<summary>Indicates data type of the 'value' key.</summary>
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        //<summary>Indicates the value corresponding to the predicate type. (Ex: RELEASED)</summary>
        [JsonPropertyName("Value")]
        public string Value { get; set; }

    }

    public class Event
    {
        //<summary>The type on which an event was performed Ex : “Document”</summary>
        [JsonPropertyName("eventClass")]
        public string EventClass { get; set; }

        //<summary>The action performed on the type. here "computed"</summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        //<summary>The number of milliseconds since January 1, 1970 (Milliseconds since epoch)</summary>
        [JsonPropertyName("eventTime")]
        public long EventTime { get; set; }

        //<summary>Represents the user name responsible for the event/action</summary>
        [JsonPropertyName("user")]
        public string User { get; set; }

        //<summary>User authorization details. Check service documentation for contextual information</summary>
        [JsonPropertyName("authorization")]
        public string Authorization { get; set; }

        //<summary>Metadata information about the created resource or reference to a computation result resource
        // or still metadata information about the resource for which the maturity change happened.</summary>
        [JsonPropertyName("subject")]
        public EventSubject Subject { get; set; }
    }
}
