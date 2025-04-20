using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HexPositionDictionaryConverter<TValue> : JsonConverter<Dictionary<HexPosition, TValue>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<HexPosition, TValue> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            var key = $"{kvp.Key.Q},{kvp.Key.R}";
            writer.WritePropertyName(key);
            serializer.Serialize(writer, kvp.Value);
        }
        writer.WriteEndObject();
    }

    public override Dictionary<HexPosition, TValue> ReadJson(JsonReader reader, Type objectType, Dictionary<HexPosition, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var result = new Dictionary<HexPosition, TValue>();
        var jObject = JObject.Load(reader);

        foreach (var property in jObject.Properties())
        {
            var keyParts = property.Name.Split(',');
            if (keyParts.Length < 2) continue;

            int q = int.Parse(keyParts[0]);
            int r = int.Parse(keyParts[1]);
            var key = new HexPosition(q, r);

            var value = property.Value.ToObject<TValue>(serializer);
            result.Add(key, value);
        }

        return result;
    }
}