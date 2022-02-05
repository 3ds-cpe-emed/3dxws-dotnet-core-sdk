
using System.Text.Json.Serialization;

using ds.enovia.common.interfaces;

namespace ds.enovia.common.model
{
    public class ItemResponseSchema : ResponseSchema, IItem
    {
        private const string ID           = "id";
        private const string TYPE         = "type";
        private const string CREATED      = "created";
        private const string MODIFIED     = "modified";
        private const string NAME         = "name";
        private const string DESCRIPTION  = "description";
        private const string TITLE        = "title";
        private const string REVISION     = "revision";
        private const string STATE        = "state";
        private const string ORGANIZATION = "organization";
        private const string OWNER        = "owner";
        private const string COLLABSPACE  = "collabspace";
        private const string CESTAMP      = "cestamp";

        [JsonPropertyName(ID)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; set; }

        [JsonPropertyName(TYPE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; set; }

        [JsonPropertyName(CREATED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Created { get; set; }

        [JsonPropertyName(MODIFIED)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Modified { get; set; }

        [JsonPropertyName(NAME)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        [JsonPropertyName(DESCRIPTION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; } 

        [JsonPropertyName(TITLE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Title { get; set; }

        [JsonPropertyName(REVISION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Revision { get; set; }

        [JsonPropertyName(STATE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string State { get; set; }

        [JsonPropertyName(OWNER)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Owner { get; set; }

        [JsonPropertyName(ORGANIZATION)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Organization { get; set; }

        [JsonPropertyName(COLLABSPACE)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Collabspace { get; set; }

        [JsonPropertyName(CESTAMP)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Cestamp { get; set; }
    }
}
