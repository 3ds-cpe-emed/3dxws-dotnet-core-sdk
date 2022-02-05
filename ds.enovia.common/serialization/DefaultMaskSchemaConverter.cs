using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ds.enovia.common.serialization
{
    /// <summary>
    /// Holds the logic to find a deserializer by mask name and type of object when read from the json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultMaskSchemaConverter<T> : MaskJson3DXSchemaConverter<T>
    {
        public DefaultMaskSchemaConverter(string name)
        {
            MaskName = name;
        }

        public override string MaskName { get; protected set; }


        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }


        public static bool TryGetTypePropertyValue(ref Utf8JsonReader _reader, out string __type)
        {
            __type = null;

            if (_reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Invalid Json - expecting Object");
            }

            while (_reader.Read())
            {
                if (_reader.TokenType == JsonTokenType.EndObject)
                {
                    return false;
                }

                if (_reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Invalid Json - expecting Property");
                }

                string propertyName = _reader.GetString();

                if (propertyName.Equals("type"))
                {
                    if (!_reader.Read())
                        throw new JsonException("");

                    if (_reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException("Invalid Json - type property should be a string");
                    }
                    __type = _reader.GetString();

                    return true;
                }

                _reader.Skip();
            }

            return false;
        }

        /// <summary>
        /// Overrides JsonConverter with custom deserialization for ItemResponseSchema and derived entities.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T __item = default(T);

            try
            {
                // TODO: Input validation

                Utf8JsonReader readerType = reader;

                string typePropertyName;

                if (!TryGetTypePropertyValue(ref readerType, out typePropertyName))
                {
                    throw new Exception("Missing type attribute from Json");
                }

                // ...

                Type t = GlobalSchemaAttributeRegistry.GetTypeFromPropertyNameAndMaskAttributes(MaskName, "type", typePropertyName);

                if (null == t)
                    throw new TypeLoadException("Cannot find type attribute or known value.");

                // processing

                __item = (T)Activator.CreateInstance(t);

                // Do Type Properties

                PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                if ((properties != null) && (properties.Length > 0))
                {
                    ReadProperties(__item, ref reader, properties);
                }
            }
            catch (Exception _ex)
            {
                //TODO: Handle/log exception
                __item = default(T);
            }

            return __item;
        }


        private void ReadProperties(object _item, ref Utf8JsonReader _reader, PropertyInfo[] _properties)
        {
            if (_reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Invalid Json - expecting Object");
            }

            while (_reader.Read())
            {
                if (_reader.TokenType == JsonTokenType.EndObject)
                {
                    return;
                }

                if (_reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Invalid Json - expecting Property");
                }

                string jPropName = _reader.GetString();

                bool skip = true;

                foreach (PropertyInfo property in _properties)
                {
                    string jPropNameParsed = property.Name;

                    //Check if the property has a JSONPropertyName attribute assigned and used it instead.
                    JsonPropertyNameAttribute jsonPropertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    if (jsonPropertyNameAttribute != null)
                        jPropNameParsed = jsonPropertyNameAttribute.Name;

                    if (!jPropName.Equals(jPropNameParsed))
                    {
                        continue;
                    };

                    // .....                           
                    skip = false;
                    SetItemValue(ref _reader, _item, property);
                    break;
                }

                if (skip)
                {
                    _reader.Skip();
                }
            }
        }

        #region Copying values from Json into DotNet instances

        private void SetItemValue(ref Utf8JsonReader _reader, object _item, PropertyInfo _property)
        {
            if (_reader.TokenType == JsonTokenType.PropertyName)
                _reader.Read();

            switch (_reader.TokenType)
            {
                case JsonTokenType.None:
                    break;

                case JsonTokenType.Null:
                    break;

                case JsonTokenType.StartArray:
                    //to be tested
                    _property.SetValue(_item, JsonSerializer.Deserialize(ref _reader, _property.PropertyType));
                    break;

                case JsonTokenType.String:
                    _property.SetValue(_item, _reader.GetString());
                    break;

                case JsonTokenType.Number:
                    if (_property.PropertyType == typeof(double))
                    {
                        _property.SetValue(_item, _reader.GetDouble());
                    }
                    else
                    {
                        if (_property.PropertyType == typeof(float))
                        {
                            _property.SetValue(_item, _reader.GetSingle());
                        }
                        else
                        {
                            if (_property.PropertyType == typeof(decimal))
                            {
                                _property.SetValue(_item, _reader.GetDecimal());
                            }
                            else
                            {
                                if (_property.PropertyType == typeof(long))
                                {
                                    _property.SetValue(_item, _reader.GetInt64());
                                }
                                else
                                {
                                    if (_property.PropertyType == typeof(int))
                                    {
                                        _property.SetValue(_item, _reader.GetInt32());
                                    }
                                    else
                                    {
                                        if (_property.PropertyType == typeof(short))
                                        {
                                            _property.SetValue(_item, _reader.GetInt16());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case JsonTokenType.True:
                    _property.SetValue(_item, true);
                    break;
                case JsonTokenType.False:
                    _property.SetValue(_item, false);
                    break;
                case JsonTokenType.StartObject:
                    //to be tested
                    _property.SetValue(_item, JsonSerializer.Deserialize(ref _reader, _property.PropertyType));
                    break;
            }
        }
        #endregion
    }
}
