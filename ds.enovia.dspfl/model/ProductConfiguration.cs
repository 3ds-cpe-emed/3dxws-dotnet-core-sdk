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
    public class ProductConfiguration
    {
        // example: My name
        // object name
        public string name { get; set; }

        // example: Front Seat
        public string title { get; set; }

        // example: My description
        // Object description value
        public string description { get; set; }

        // example: EE562168015FFCF14F940A513C63AA77
        // Entity physical id
        public string id { get; set; }

        // example: 3DSpaceURL
        public string source { get; set; }        

        // example: EE5211D2015FF9F94F940A513C63AA77
        public string identifier { get; set; }

        // example: resources/v1/modeler/dspfl/dspfl:ModelVersion/MM562168015FFCF14F940A513C63BB77/dspfl:ProductConfiguration/EE562168015FFCF14F940A513C63AA77
        public string relativePath { get; set; }
    
        // example: My Type
        // Basic type value
        public string type { get; set; }

        // example: Dec 15, 2017 11:17 PM
        // Basic modified value
        public string modified { get; set; }
    
        // example: Dec 11, 2017 12:53 PM
        // Object created value
        public string created { get; set; }
    
        // example: A.1
        // Object revision value
        public string revision { get; set; }

        // example: In Work
        // Object current state value
        public string state { get; set; }

        // example: John Doe
        // Object owner value
        public string owner { get; set; }

        // example: MyCompany
        // Object organization value

        public string organization { get; set; }
        
        //example: Default
        //Object collabspace value
        public string collabspace { get; set; }

        public string completenessStatus { get; set; }
        public string evalRules { get; set; }
        public string multiSelection { get; set; }
        public string compliancyStatus { get; set; }
        public string evalMode { get; set; }
        public string selectMode { get; set; }
        public string listPrice { get; set; }

    }
}
