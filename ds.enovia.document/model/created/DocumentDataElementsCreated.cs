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

namespace ds.enovia.document.model
{
    public class DocumentDataElementsCreated
    {
        public string name { get; set; }
        public string policy { get; set; }
        public string state { get; set; }
        public string stateNLS { get; set; }
        public string typeNLS { get; set; }
        public string revision { get; set; }

        public string isLatestRevision { get; set; }

        public string title { get; set; }
        public string description { get; set; }
        public string collabspace { get; set; }

        public string originated { get; set; }
        public string modified { get; set; }

        public string comments { get; set; }
        public string hasDownloadAccess { get; set; }
        public string hasReviseAccess { get; set; }
        public string hasModifyAccess { get; set; }
        public string hasDeleteAccess { get; set; }
        public string reservedby { get; set; }
        public string secondaryTitle { get; set; }
        public string typeicon { get; set; }
        public string image { get; set; }
        public string parentId { get; set; }
    
    }
}
