using System;
using System.Collections.Generic;
using System.Text;

namespace ds.delmia.dsmfg.interfaces
{
    // outsourced is not an attribute of the Installation type
    // Create Failed Attribute 'DELAsmLotRefRequired.V_LotNumberRequired' does not exist.
    // Create Failed Attribute 'DELAsmUnitRefRequired.V_SerialNumberRequired' does not exist.

    // Supports only the PlanningRequired
    public interface IManufacturingInstallationData : IPlanningRequired
    {
        IDictionary<string, object> ManufacturingInstallationEnterpriseAttributes { get; set; }
    }
}
