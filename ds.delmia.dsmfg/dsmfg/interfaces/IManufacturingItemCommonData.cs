using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    public interface IManufacturingItemCommonData : IManufacturingItemBaseData, IOutsourced, IPlanningRequired, ILotNumberRequired, ISerialNumberRequired
    {
    }
}
