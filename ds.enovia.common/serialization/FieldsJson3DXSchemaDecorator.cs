using System;
using System.Text.Json;

namespace ds.enovia.common.serialization
{
    public class FieldsJson3DXSchemaDecorator<T> : Json3DXSchemaConverter<T>
    {
        Json3DXSchemaConverter<T> m_converter;

        public string FieldsName { get; protected set; }

        public FieldsJson3DXSchemaDecorator(Json3DXSchemaConverter<T> _converter)
        {
            m_converter = _converter;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return m_converter.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
