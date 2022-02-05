using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    [MaskSchema(MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS)]
    [ItemTypeSchema(MFGResourceNames.PROVIDED_PART_TYPE)]

    public class ProvidedPartDetail : ManufacturingItem, IProvidedPartDetail
    {
        [JsonPropertyName(MFGResourceNames.PROVIDED_PART_ENTERPRISE_ATTS)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> ProvidedPartEnterpriseAttributes { get; set; }

        [JsonPropertyName(MFGResourceNames.DSMFG_IS_LOT_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsLotNumberRequired { get ; set; }

        [JsonPropertyName(MFGResourceNames.DSMFG_IS_SERIAL_NUMBER_REQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsSerialNumberRequired { get; set; }
    }
}
