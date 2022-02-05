using ds.delmia.dsmfg.interfaces;
using ds.enovia.common.model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    public abstract class ManufacturingItemBaseCreate : ItemCreate, IManufacturingItemBaseCreate
    {
        public ManufacturingItemBaseCreate(string _type = null) : base(_type)
        { }

        [JsonPropertyName(MFGResourceNames.DSMFG_MFGENTERPRISEATTRIBUTES)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> MfgItemEnterpriseAttributes { get; set; }
    }
}
