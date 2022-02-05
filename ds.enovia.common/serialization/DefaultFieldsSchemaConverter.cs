﻿using System;
using System.Reflection;
using System.Text.Json;

namespace ds.enovia.common.serialization
{
    public class DefaultFieldsSchemaConverter<T> : FieldsJson3DXSchemaDecorator<T>
    {
        public DefaultFieldsSchemaConverter(string fieldsName, Json3DXSchemaConverter<T> _converter) : base(_converter)
        {
            FieldsName = fieldsName;
        }

        
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader fieldsReader = reader;

            T baseInstance = base.Read(ref reader, typeToConvert, options);

            Type fieldsType = GlobalSchemaAttributeRegistry.GetFieldsSchemaType(FieldsName);

            if (null == fieldsType) return baseInstance;

            IFieldsSchema _fieldsSchemaInstance = (IFieldsSchema)Activator.CreateInstance(fieldsType);

            DecorateBaseInstance(baseInstance, ref fieldsReader, _fieldsSchemaInstance, options);

            return baseInstance;
        }

        /// <summary>
        /// Decorates deserialize base instance with fields schema logic
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_reader"></param>
        /// <param name="_fields"></param>
        private void DecorateBaseInstance(object _item, ref Utf8JsonReader _reader, IFieldsSchema _fields, JsonSerializerOptions options)
        {
            string _propName = _fields.PropertyName;

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

                if (!jPropName.Equals(_propName))
                {
                    _reader.Skip();
                    continue; //READ NEXT
                };

                SetItemValue(ref _reader, _item, _fields, options);
                break;
            }
        }

        private void SetItemValue(ref Utf8JsonReader _reader, object _item, IFieldsSchema _fields, JsonSerializerOptions options)
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
                    _fields.SetValue(_item, JsonSerializer.Deserialize(ref _reader, _fields.PropertyType, options));
                    break;

                case JsonTokenType.String:
                    _fields.SetValue(_item, _reader.GetString());
                    break;

                case JsonTokenType.Number:
                    Type fieldPropertyType = _fields.PropertyType;

                    if (fieldPropertyType == typeof(double))
                    {
                        _fields.SetValue(_item, _reader.GetDouble());
                    }
                    else
                    {
                        if (fieldPropertyType == typeof(float))
                        {
                            _fields.SetValue(_item, _reader.GetSingle());
                        }
                        else
                        {
                            if (fieldPropertyType == typeof(decimal))
                            {
                                _fields.SetValue(_item, _reader.GetDecimal());
                            }
                            else
                            {
                                if (fieldPropertyType == typeof(long))
                                {
                                    _fields.SetValue(_item, _reader.GetInt64());
                                }
                                else
                                {
                                    if (fieldPropertyType == typeof(int))
                                    {
                                        _fields.SetValue(_item, _reader.GetInt32());
                                    }
                                    else
                                    {
                                        if (fieldPropertyType == typeof(short))
                                        {
                                            _fields.SetValue(_item, _reader.GetInt16());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case JsonTokenType.True:
                    _fields.SetValue(_item, true);
                    break;
                case JsonTokenType.False:
                    _fields.SetValue(_item, false);
                    break;
                case JsonTokenType.StartObject:
                    //to be tested
                    _fields.SetValue(_item, JsonSerializer.Deserialize(ref _reader, _fields.PropertyType, options));
                    break;
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
