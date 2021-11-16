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

namespace ds.enovia.dseng.model
{
    public class EngineeringInstance : SerializableJsonObject
    {
        //Entity physical id
        //example: EE562168015FFCF14F940A513C63AA77
        public string id { get; set; }

        //Basic type value
        //example: My Type
        public string type { get; set; }

        // Basic modified value
        // example: Dec 15, 2017 11:17 PM
        public string modified { get; set; }

        // Object created value
        // example: Dec 11, 2017 12:53 PM
        public string created { get; set; }

        // Instance name
        // example: My name
        public string name { get; set; }

        // Instance description value
        // example: My description
        public string description { get; set; }

        // Object cestamp value
        // example: 2D70169432D84866A200F907881AC9B1
        public string cestamp { get; set; }
    }
}
