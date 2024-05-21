using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartTripPlanner.Core.Graph;

namespace SmartTripPlanner.Core.JsonConverters;
public class StronglyTypedVertexIdJsonConverter<T> : JsonConverter<T>
    where T : StronglyTypedVertexId, new()
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException("Reader.GetString() was null.");
        }

        return new T() { Value = value };
    }

    // Crucial for serialize/deserialize it when it is a dictionary key.
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);

    // Crucial for serialize/deserialize it when it is a dictionary key.
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WritePropertyName(value.Value);
}
