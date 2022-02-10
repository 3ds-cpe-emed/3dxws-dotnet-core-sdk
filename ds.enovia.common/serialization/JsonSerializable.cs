// ------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2022 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
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
