using System;
using System.Globalization;
using Newtonsoft.Json;

namespace InfoEducatie.Base;

public class TimeOnlyConverter : JsonConverter
{
    public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer)
    {
        try
        {
            bool result = TimeOnly.TryParse(reader?.Value?.ToString(), out TimeOnly time);
            if (result)
            {
                return time;
            }
        }
        catch { }
        return Activator.CreateInstance(type);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(((TimeOnly?)value)?.ToString("HH:mm", CultureInfo.InvariantCulture));
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(TimeOnly) || type == typeof(TimeOnly?);
    }
}
