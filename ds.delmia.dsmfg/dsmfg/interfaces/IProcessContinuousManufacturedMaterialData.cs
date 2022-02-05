using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IProcessContinuousManufacturedMaterialData
    {        
        IDictionary<string, object> PCManufacturedMaterialEnterpriseAttributes { get; set; }
    }

}
