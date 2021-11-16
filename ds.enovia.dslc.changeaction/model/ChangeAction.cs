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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.enovia.dslc.changeaction.model
{
    public class ChangeAction
    {
        public string id { get; set;} //physical id
        public string cestamp { get; set;}
        public string policy { get; set; } //e.g. "Change Action"
        public string description { get; set; } //
        public string title { get; set; } //e.g. "CA - some summary title"
        public string name { get; set; } //e.g. "CA-36236428-00000006"
        public string state { get; set; } //e.g."In Work"
        public string owner { get; set; }
        public string organization { get; set; }
        public string collabSpace { get; set; }
        public string severity { get; set; }

        [JsonPropertyName("Estimated Start Date")]
        public string estimatedStartDate { get; set; }
        [JsonPropertyName("Actual Start Date")]
        public string actualStartDate { get; set; }
        [JsonPropertyName("Estimated Completion Date")]
        public string estimatedCompletionDate { get; set; }
        [JsonPropertyName("Actual Completion Date")]

        public string actualCompletionDate { get; set; }
        public string xmlApplicability { get; set; }
        public bool onHold { get; set; }
        public ChangeActionMembers members { get; set; }
        public List<ChangeActionProposedChange> proposedChanges { get; set; }
        public List<ChangeActionRealizedChange> realizedChanges { get; set; }
        public List<object> referentials { get; set; }
        public List<object> contexts { get; set; }
    }
}
