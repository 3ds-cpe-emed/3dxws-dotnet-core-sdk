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

using ds.enovia.common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.enovia.dseng.model
{
    public class EngineeringItemPatch : SerializableJsonObject
    {
        public EngineeringItemPatch(EngineeringItem _item)
        {
            this.title = _item.title;
            this.description = _item.description;
            this.cestamp = _item.cestamp;
            this.enterpriseAttributes = _item.enterpriseAttributes;
            this.enterpriseReference = _item.enterpriseReference;
        }

        public string title { get; set; }
        public string description { get; set; }
        public string cestamp { get; set; }

        [JsonPropertyName("dseng:EnterpriseReference")]
        public EnterpriseReference enterpriseReference { get; set; }

        [JsonPropertyName("dseno:EnterpriseAttributes")]
        public Dictionary<string, object> enterpriseAttributes { get; set; }
    }
}
