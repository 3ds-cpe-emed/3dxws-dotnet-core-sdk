using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURED_PART_TYPE)]
    public class ManufacturedPartDetail : ManufacturingItemDetail, IManufacturedPartDetail
    {
        [JsonPropertyName(MFGResourceNames.MANUFACTURED_PART_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> ManufacturedPartEnterpriseAttributes { get; set; }
    }
}
