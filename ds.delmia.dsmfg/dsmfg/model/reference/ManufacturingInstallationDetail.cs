using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURING_INSTALLATION_TYPE)]
    public class ManufacturingInstallationDetail : ManufacturingItem, IManufacturingInstallationDetail
    {
        [JsonPropertyName(MFGResourceNames.MANUFACTURING_INSTALLATION_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> ManufacturingInstallationEnterpriseAttributes { get; set; }

        [JsonPropertyName(MFGResourceNames.DSMFG_PLANNING_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PlanningRequired { get; set; }
    }
}
