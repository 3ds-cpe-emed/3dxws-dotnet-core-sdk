using ds.delmia.dsmfg.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.model.reference
{
    public class ManufacturingItemDetail : ManufacturingItem, IManufacturingItemDetail
    {
        private const string OUTSOURCED = "outsourced";
        private const string PLANNINGREQUIRED = "planningRequired";
        private const string ISLOTNUMBERREQUIRED = "isLotNumberRequired";
        private const string ISSERIALNUMBERREQUIRED = "isSerialNumberRequired";

        [JsonPropertyName(OUTSOURCED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Outsourced { get; set; }
        [JsonPropertyName(PLANNINGREQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PlanningRequired { get; set; }
        [JsonPropertyName(ISLOTNUMBERREQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsLotNumberRequired { get; set; }
        [JsonPropertyName(ISSERIALNUMBERREQUIRED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsSerialNumberRequired { get; set; }

        [JsonPropertyName(MFGResourceNames.DSMFG_MFGENTERPRISEATTRIBUTES)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object> MfgItemEnterpriseAttributes { get; set; }

    }
}
