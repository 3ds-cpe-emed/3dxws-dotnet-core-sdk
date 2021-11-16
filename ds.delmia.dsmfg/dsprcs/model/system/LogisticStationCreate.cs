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
using System.Text;

namespace ds.delmia.dsprcs.model.system
{
    // Verify: Currently it seems that, even if can create them, we cannot read LogisticStations through the Manufacturing Process APIs
    // 404 error (URI not Found)

    public class LogisticStationCreate : ManufacturingProcessCreate
    {
        public LogisticStationCreate()
        {
            this.attributes = new LogisticStationEnterpriseAttributes();
            this.attributes.Add(TYPE, MFGResourceNames.LOGISTIC_STATION[0]);
        }
    }
    public class LogisticStationEnterpriseAttributes : MfgProcessEnterpriseAttributes
    {
        private readonly string CUSTOM_ENTERPRISEATTRIBUTES = MFGResourceNames.LOGISTIC_STATION[1];
        public LogisticStationEnterpriseAttributes()
        { }
    }
}
