using ds.enovia.common.serialization;
using System;
using System.Collections.Generic;

namespace ds.enovia.common.fields
{
    [FieldsSchema("dsmvcfg:attribute.isConfigured")]
    public class ConfiguredAttributeFields : IFieldsSchema
    {
        private const string FIELDS_ATTRIBUTE_NAME = "dscfg:Configured";

        public string PropertyName => FIELDS_ATTRIBUTE_NAME;

        public Type PropertyType => typeof(Dictionary<string,object>);

        public void SetValue(object item, object value)
        {
            ((dynamic)item).isConfigured = value;
        }
    }
}
