//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
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
//------------------------------------------------------------------------------------------------------------------------------------

using ds.delmia.dsmfg.model;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ds.delmia.dsmfg.converter
{

    public class ManufacturingItemDetailsConverter : JsonConverter<ManufacturingItemDetails>
    {
        private static PropertyInfo[] m_properties = typeof(ManufacturingItemDetails).GetProperties();

        //public override void WriteJson(JsonWriter writer, ManufacturingItemDetails value, JsonSerializer serializer)
        //{
        //    writer.WriteValue(value.ToString());
        //}

        //public override ManufacturingItemDetails ReadJson(JsonReader reader, Type objectType, ManufacturingItemDetails existingValue, bool hasExistingValue, JsonSerializer serializer)
        //{
        //    ManufacturingItemDetails __mfgItemEnterpriseAtts = new ManufacturingItemDetails();

        //    JObject jObject = (JObject)JToken.ReadFrom(reader);

        //    foreach (JToken token in jObject.Children())
        //    {
        //        if (token.Type == JTokenType.Property)
        //        {
        //            bool found = false;

        //            string jpropName = ((JProperty)token).Name;

        //            foreach (PropertyInfo property in m_properties)
        //            {
        //                if (!property.Name.Equals(jpropName)) continue;
                        
        //                if (!property.Name.Equals("interfaces"))
        //                {
        //                    if (property.PropertyType == typeof(string))
        //                    {
        //                        property.SetValue(__mfgItemEnterpriseAtts, ((JValue)((JProperty)token).Value).Value.ToString());
        //                    }
        //                    else
        //                    {
        //                        property.SetValue(__mfgItemEnterpriseAtts, ((JValue)((JProperty)token).Value).Value);
        //                    }

        //                    found = true;
        //                }
        //            }

        //            if (!found)
        //            {
        //                try
        //                {
        //                    object jprop = (((JProperty)token).Value);

        //                    Type deserializeType = Type.GetType("ds.delmia.dsmfg.model." + jpropName);

        //                    __mfgItemEnterpriseAtts.interfaces.Add(jpropName, JsonConvert.DeserializeObject(jprop.ToString(), deserializeType));
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }

        //    return __mfgItemEnterpriseAtts;
        //}

        public override ManufacturingItemDetails Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ManufacturingItemDetails __mfgItemEnterpriseAtts = new ManufacturingItemDetails();

            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                JsonElement element = document.RootElement;

                JsonElement.ObjectEnumerator jsonPropertiesEnumerator = element.EnumerateObject();

                while (jsonPropertiesEnumerator.MoveNext())
                {
                    JsonProperty testProperty = jsonPropertiesEnumerator.Current;

                    bool found = false;

                    string jpropName = testProperty.Name;

                    foreach (PropertyInfo property in m_properties)
                    {
                        if (!property.Name.Equals(jpropName)) continue;

                        if (!property.Name.Equals("interfaces"))
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(__mfgItemEnterpriseAtts,  testProperty.Value.ToString());
                            }
                            else
                            {
                                object val = null;

                                if (property.PropertyType == typeof(long))
                                {
                                    val = testProperty.Value.GetInt64();
                                }

                                if (property.PropertyType == typeof(bool))
                                {
                                    val = testProperty.Value.GetBoolean();
                                }

                                if (property.PropertyType == typeof(double))
                                {
                                    val = testProperty.Value.GetDouble();
                                }

                                property.SetValue(__mfgItemEnterpriseAtts, val);
                            }

                            found = true;
                        }
                    }

                    if (!found)
                    {
                        try
                        {
                            object jprop = testProperty.Value;

                            Type deserializeType = Type.GetType("ds.delmia.dsmfg.model." + jpropName);

                            __mfgItemEnterpriseAtts.interfaces.Add(jpropName, JsonSerializer.Deserialize(jprop.ToString(), deserializeType));
                        }
                        catch
                        {
                            //TODO:
                        }
                    }
                }
            }

            return __mfgItemEnterpriseAtts;
        }

        public override void Write(Utf8JsonWriter writer, ManufacturingItemDetails value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.toJson());
        }
    }
}
