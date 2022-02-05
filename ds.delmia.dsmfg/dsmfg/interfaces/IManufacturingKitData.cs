using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IManufacturingKitData
    {
        IDictionary<string, object> ManufacturingKitEnterpriseAttributes { get; set; }
    }
}
