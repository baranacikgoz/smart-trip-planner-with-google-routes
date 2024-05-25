using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using SmartTripPlanner.ChargePoints.Models;

namespace SmartTripPlanner.ChargePoints.JsonConverters;
public class ChargePointJsonConverter : JsonConverter<ChargePoint>
{
    public override ChargePoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var jsonObject = doc.RootElement;
            var barcode = jsonObject.GetProperty("Barcode").GetString() ?? throw new InvalidOperationException("Barcode is null.");
            var latitude = jsonObject.GetProperty("Latitude").GetDouble();
            var longitude = jsonObject.GetProperty("Longitude").GetDouble();
            var isDc = jsonObject.GetProperty("IsDc").GetBoolean();

            return new ChargePoint(new ChargePointBarcode(barcode), latitude, longitude, isDc);
        }
    }

    public override void Write(Utf8JsonWriter writer, ChargePoint value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Barcode", value.Barcode.Value);
        writer.WriteNumber("Latitude", value.Latitude);
        writer.WriteNumber("Longitude", value.Longitude);
        writer.WriteBoolean("IsDc", value.IsDc);
        writer.WriteEndObject();
    }

    // Crucial for serialize/deserialize it when it is a dictionary key.
    public override ChargePoint ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var propertyName = reader.GetString();
        if (propertyName == null)
        {
            throw new JsonException("Property name is null.");
        }
        return JsonSerializer.Deserialize<ChargePoint>(propertyName, options)
            ?? throw new InvalidOperationException("Deserializer returned null.");
    }

    // Crucial for serialize/deserialize it when it is a dictionary key.
    public override void WriteAsPropertyName(Utf8JsonWriter writer, ChargePoint value, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(value, options);
        writer.WritePropertyName(json);
    }
}
