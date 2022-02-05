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

using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.model;
using ds.enovia.common.serialization;

namespace ds.delmia.dsmfg.model.reference
{
    public class ManufacturingItemCreate : ManufacturingItemBaseCreate, IManufacturingItemCreate
    {
        protected ManufacturingItemCreate(string _type = null): base(_type)
        {
        }

        [JsonPropertyName(MFGResourceNames.DSMFG_OUTSOURCED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Outsourced { get; set; }
        [JsonPropertyName(MFGResourceNames.DSMFG_PLANNING_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PlanningRequired { get; set; }
        [JsonPropertyName(MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsLotNumberRequired { get; set; }
        [JsonPropertyName(MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsSerialNumberRequired { get; set; }
    }
}
