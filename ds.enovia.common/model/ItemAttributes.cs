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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ds.enovia.common.model
{
    public class ItemAttributes
    {
        //Standard properties
        private const string TYPE         = "type";
        private const string ID           = "id";
        private const string NAME         = "name";
        private const string TITLE        = "title";
        private const string DESCRIPTION  = "description";

        private const string MODIFIED     = "modified";
        private const string CREATED      = "created";
        private const string REVISION     = "revision";
        private const string STATE        = "state";
        private const string OWNER        = "owner";
        private const string ORGANIZATION = "organization";
        private const string COLLABSPACE  = "collabspace";
        private const string CESTAMP      = "cestamp";

        private const string CUSTOMERATTRIBUTES = "customerAttributes";

        protected Dictionary<string, object> m_dictionary = null;

        public ItemAttributes()
        {
            m_dictionary = new Dictionary<string, object>();
        }

        public void Add(string _key, object _value)
        {
            m_dictionary.Add(_key, _value);
        }

        [JsonPropertyName(TYPE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string type
        {
            get { return m_dictionary.ContainsKey(TYPE) ? (string)m_dictionary[TYPE] : null; }
        }

        [JsonPropertyName(ID)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string id
        {
            get { return m_dictionary.ContainsKey(ID) ? (string)m_dictionary[ID] : null; }
        }

        [JsonPropertyName(NAME)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string name
        {
            get { return m_dictionary.ContainsKey(NAME) ? (string)m_dictionary[NAME] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[NAME] = value;
            }
        }

        [JsonPropertyName(TITLE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string title
        {
            get { return m_dictionary.ContainsKey(TITLE) ? (string)m_dictionary[TITLE] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[TITLE] = value;
            }
        }

        [JsonPropertyName(DESCRIPTION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string description
        {
            get { return m_dictionary.ContainsKey(DESCRIPTION) ? (string)m_dictionary[DESCRIPTION] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DESCRIPTION] = value;
            }
        }

        [JsonPropertyName(MODIFIED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string modified
        {
            get { return m_dictionary.ContainsKey(MODIFIED) ? (string)m_dictionary[MODIFIED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[MODIFIED] = value;
            }
        }

        [JsonPropertyName(CREATED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string created
        {
            get { return m_dictionary.ContainsKey(CREATED) ? (string)m_dictionary[CREATED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[CREATED] = value;
            }
        }

        [JsonPropertyName(REVISION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string revision
        {
            get { return m_dictionary.ContainsKey(REVISION) ? (string)m_dictionary[REVISION] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[REVISION] = value;
            }
        }

        [JsonPropertyName(STATE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string state
        {
            get { return m_dictionary.ContainsKey(STATE) ? (string)m_dictionary[STATE] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[STATE] = value;
            }
        }

        [JsonPropertyName(OWNER)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string owner
        {
            get { return m_dictionary.ContainsKey(OWNER) ? (string)m_dictionary[OWNER] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[OWNER] = value;
            }
        }

        [JsonPropertyName(ORGANIZATION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string organization
        {
            get { return m_dictionary.ContainsKey(ORGANIZATION) ? (string)m_dictionary[ORGANIZATION] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[ORGANIZATION] = value;
            }
        }

        [JsonPropertyName(COLLABSPACE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string collabspace
        {
            get { return m_dictionary.ContainsKey(COLLABSPACE) ? (string)m_dictionary[COLLABSPACE] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[COLLABSPACE] = value;
            }
        }

        [JsonPropertyName(CESTAMP)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string cestamp
        {
            get { return m_dictionary.ContainsKey(CESTAMP) ? (string)m_dictionary[CESTAMP] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[CESTAMP] = value;
            }
        }
        /// <summary>
        /// Check Customization section under https://media.3ds.com/support/documentation/developer/Cloud/en/DSDoc.htm?show=CAA3DSpaceREST/CAAXInfraWSTaOverview.htm
        /// Customer attributes can only be added now onPremise through TXO. (2022XGA)
        /// 
        /// "customerAttributes": {
        ///     "attr1": "value1",
        ///     "attr2": "value2",
        ///     "CUSTOM_Extension": {
        ///              "attr3": "value3",
        ///              "attr4": "value4"
        ///     },
        ///     "CUSTOM_AttributeGroup": {
        ///             "attr5": "value5",
        ///             "attr6": "value6"
        ///             }
        /// }
        /// </summary>
        [JsonPropertyName(CUSTOMERATTRIBUTES)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> customerAttributes
        {
            get { return m_dictionary.ContainsKey(CUSTOMERATTRIBUTES) ? (IDictionary<string, object>)m_dictionary[CUSTOMERATTRIBUTES] : null; }

            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[CUSTOMERATTRIBUTES] = value;
            }
        }
    }
}
