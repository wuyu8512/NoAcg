using System;
using Newtonsoft.Json;
using Wuyu.OneBot.Expansion;

namespace Wuyu.OneBot.Converter
{
    internal class DataTimeConverter: JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(serializer.DateFormatString));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                var val = Convert.ToInt64(reader.Value);
                return val.ToDateTime();
            }

            return reader.ReadAsDateTime() ?? default;
        }
    }
}