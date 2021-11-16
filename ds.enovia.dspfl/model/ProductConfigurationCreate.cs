using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
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

namespace ds.enovia.dspfl.model
{
    public class ProductConfigurationCreateItems 
    {
        public ProductConfigurationCreateItems()
        {
            items = new List<ProductConfigurationCreate>();
        }

        public List<ProductConfigurationCreate> items { get; set; }
    }

    public class ProductConfigurationCreateAttributes
    {
        public ProductConfigurationCreateAttributes()
        {
            title = "";
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string name { get; set; } //"PC01,
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string description { get; set; } // "PC01",

        public string title { get; set; } // "PC Marketing Name",
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string purpose { get; set; }// "Evaluation",
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string listPrice { get; set; } // "1.0" 
    }

    public class ProductConfigurationCreate
    {
        public ProductConfigurationCreate()
        {
            versionName = "";
            type = "";
            selectedCriteria = new List<Criteria>();
            attributes = new ProductConfigurationCreateAttributes();
        }

        public string versionName { get; set; }
        public string type { get; set; }
        public List<Criteria> selectedCriteria { get; set; }
        public ProductConfigurationCreateAttributes attributes { get; set; }
    }
}
