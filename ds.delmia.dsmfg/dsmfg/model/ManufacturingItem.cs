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
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model
{
    public class ManufacturingItem : Item
    {
       
        public ManufacturingItem()
        {
            m_dictionary = new Dictionary<string, object>();
            interfaces = new Dictionary<string, object>();
        }

        private Dictionary<string, object> m_dictionary;
        public Dictionary<string, object> interfaces;

        [JsonPropertyName(MFGResourceNames.DSMFG_OUTSOURCED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string outsourced
        {
            get { return m_dictionary.ContainsKey(MFGResourceNames.DSMFG_OUTSOURCED) ? (string)m_dictionary[MFGResourceNames.DSMFG_OUTSOURCED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[MFGResourceNames.DSMFG_OUTSOURCED] = value;
            }
        }

        [JsonPropertyName(MFGResourceNames.DSMFG_PLANNING_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string planningRequired
        {
            get { return m_dictionary.ContainsKey(MFGResourceNames.DSMFG_PLANNING_REQUIRED) ? (string)m_dictionary[MFGResourceNames.DSMFG_PLANNING_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[MFGResourceNames.DSMFG_PLANNING_REQUIRED] = value;
            }
        }

        [JsonPropertyName(MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? isLotNumberRequired
        {
            get { return m_dictionary.ContainsKey(MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED) ? (bool?)m_dictionary[MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED] = value;
            }
        }

        [JsonPropertyName(MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? isSerialNumberRequired
        {
            get { return m_dictionary.ContainsKey(MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED) ? (bool?)m_dictionary[MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED] : null; }
            set
            {
                //Note that if key doesn't exist gets created
                m_dictionary[MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED] = value;
            }
        }
    }
}
