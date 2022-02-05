using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IManufacturedPartData
    {
        IDictionary<string, object> ManufacturedPartEnterpriseAttributes { get; set; }
    }
}
