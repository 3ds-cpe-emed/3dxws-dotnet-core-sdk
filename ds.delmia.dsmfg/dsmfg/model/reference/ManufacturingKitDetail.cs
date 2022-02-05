using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURING_KIT_TYPE)]
    public class ManufacturingKitDetail : ManufacturingItemDetail, IManufacturingKitDetail
    {
        [JsonPropertyName(MFGResourceNames.MANUFACTURING_KIT_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> ManufacturingKitEnterpriseAttributes { get; set; }
    }
}
