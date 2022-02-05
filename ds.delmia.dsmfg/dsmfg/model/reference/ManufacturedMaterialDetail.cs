using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.MANUFACTURED_MATERIAL_TYPE)]
    public class ManufacturedMaterialDetail : ManufacturingItemDetail, IManufacturedMaterialDetail
    {
        [JsonPropertyName(MFGResourceNames.MANUFACTURED_MATERIAL_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> CreateMaterialEnterpriseAttributes { get; set; }

    }
}
