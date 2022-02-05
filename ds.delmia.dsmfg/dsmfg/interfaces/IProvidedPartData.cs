using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    // Supports only IsLotNumber and IsSerialNumber
    public interface IProvidedPartData : ILotNumberRequired, ISerialNumberRequired
    {
        IDictionary<string, object> ProvidedPartEnterpriseAttributes { get; set; }
    }
 
}
