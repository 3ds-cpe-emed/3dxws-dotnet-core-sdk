using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURING_ASSEMBLY_TYPE)]
    public class ManufacturingAssemblyDetail : ManufacturingItemDetail, IManufacturingAssemblyDetail
    {
        [JsonPropertyName(MFGResourceNames.MANUFACTURING_ASSEMBLY_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> ManufacturingAssemblyEnterpriseAttributes { get; set; }
    }
}
