using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ds.enovia.common.serialization
{
    public class JsonSerializableConverter<T> : JsonConverter<T> where T : JsonSerializable
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            value.Write(writer, value, options);
        }
    }
}
