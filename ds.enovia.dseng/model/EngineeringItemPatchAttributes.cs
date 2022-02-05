﻿//------------------------------------------------------------------------------------------------------------------------------------
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

using ds.enovia.common.model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.enovia.dseng.model
{
    public class EngineeringItemPatchAttributes : ItemAttributes
    {
        private const string ENTERPRISE_ATTS = "dseno:EnterpriseAttributes";

        [JsonPropertyName(ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> EnterpriseAttributes
        {
            get { return m_dictionary.ContainsKey(ENTERPRISE_ATTS) ? (IDictionary<string, object>)m_dictionary[ENTERPRISE_ATTS] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[ENTERPRISE_ATTS] = value;
            }
        }
    }
}