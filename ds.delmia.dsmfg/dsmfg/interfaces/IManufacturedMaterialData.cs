using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IManufacturedMaterialData
    {
        IDictionary<string, object> CreateMaterialEnterpriseAttributes { get; set; }
    }
}
