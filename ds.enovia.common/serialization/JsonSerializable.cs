using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ds.enovia.common.serialization
{
    public class JsonSerializable
    {
        public virtual void Write(Utf8JsonWriter writer, JsonSerializable value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            PropertyInfo[] thisInstanceProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in thisInstanceProperties)
            {
                // json property type
                Type propertyType = property.PropertyType;

                //TODO: REVIEW
                if (propertyType.IsSubclassOf(typeof(JsonSerializable)))
                {
                    JsonSerializable jsonPropertyValue = (JsonSerializable)property.GetValue(this);
                    jsonPropertyValue.Write(writer, jsonPropertyValue, options);
                    continue;
                }

                // json propertyName
                string propertyName     = property.Name;
                string jsonPropertyName = propertyName;

                //Check if property has a Jsonserializable attribute
                JsonPropertyNameAttribute jsonPropertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();

                if (jsonPropertyNameAttribute != null)
                {
                    jsonPropertyName = jsonPropertyNameAttribute.Name;
                }

                JsonIgnoreAttribute jsonIgnoreAttribute = property.GetCustomAttribute<JsonIgnoreAttribute>();

                object propertyValue = property.GetValue(this);

                if (jsonIgnoreAttribute != null)
                {
                    if (jsonIgnoreAttribute.Condition == JsonIgnoreCondition.WhenWritingNull)
                    {
                        if (propertyValue != null)
                        {
                            writer.WritePropertyName(jsonPropertyName);
                            JsonSerializer.Serialize(writer, propertyValue, propertyType, options);
                        }
                        continue;
                    }
                }

                writer.WritePropertyName(jsonPropertyName);
                JsonSerializer.Serialize(writer, propertyValue, propertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}
