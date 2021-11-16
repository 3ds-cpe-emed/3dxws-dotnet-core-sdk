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

using ds.enovia.common.model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model
{
    public class MfgItemEnterpriseAttributes : ItemAttributes
    {
        protected const string DSMFG_MFGENTERPRISEATTRIBUTES   = "dsmfg:MfgItemEnterpriseAttributes";
        protected const string DSMFG_OUTSOURCED                = "outsourced";
        protected const string DSMFG_PLANNING_REQUIRED         = "planningRequired";
        protected const string DSMFG_IS_LOT_NUMBER_REQUIRED    = "isLotNumberRequired";
        protected const string DSMFG_IS_SERIAL_NUMBER_REQUIRED = "isSerialNumberRequired";

        public MfgItemEnterpriseAttributes()
        { }

        [JsonPropertyName(DSMFG_MFGENTERPRISEATTRIBUTES)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> dsmfg_MfgItemEnterpriseAttributes
        {
            get { return m_dictionary.ContainsKey(DSMFG_MFGENTERPRISEATTRIBUTES) ? (IDictionary<string, object>)m_dictionary[DSMFG_MFGENTERPRISEATTRIBUTES] : null; }

            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DSMFG_MFGENTERPRISEATTRIBUTES] = value;
            }
        }

        [JsonPropertyName(DSMFG_OUTSOURCED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string outsourced
        {
            get { return m_dictionary.ContainsKey(DSMFG_OUTSOURCED) ? (string)m_dictionary[DSMFG_OUTSOURCED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DSMFG_OUTSOURCED] = value;
            }
        }

        [JsonPropertyName(DSMFG_PLANNING_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string planningRequired
        {
            get { return m_dictionary.ContainsKey(DSMFG_PLANNING_REQUIRED) ? (string)m_dictionary[DSMFG_PLANNING_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DSMFG_PLANNING_REQUIRED] = value;
            }
        }

        [JsonPropertyName(DSMFG_IS_LOT_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string isLotNumberRequired
        {
            get { return m_dictionary.ContainsKey(DSMFG_IS_LOT_NUMBER_REQUIRED) ? (string)m_dictionary[DSMFG_IS_LOT_NUMBER_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DSMFG_IS_LOT_NUMBER_REQUIRED] = value;
            }
        }

        [JsonPropertyName(DSMFG_IS_SERIAL_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string isSerialNumberRequired
        {
            get { return m_dictionary.ContainsKey(DSMFG_IS_SERIAL_NUMBER_REQUIRED) ? (string)m_dictionary[DSMFG_IS_SERIAL_NUMBER_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[DSMFG_IS_SERIAL_NUMBER_REQUIRED] = value;
            }
        }
    }
}
