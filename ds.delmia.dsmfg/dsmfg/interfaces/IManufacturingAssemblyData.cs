using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IManufacturingAssemblyData
    {
        IDictionary<string, object> ManufacturingAssemblyEnterpriseAttributes { get; set; }
    }
}
