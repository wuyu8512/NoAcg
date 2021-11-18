using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace NoAcgNew.Converter
{
    public class WebProxyJsonConverter : JsonConverter<IWebProxy>
    {
        public override bool CanWrite { get; } = false;

        public override IWebProxy ReadJson(JsonReader reader, Type objectType, IWebProxy existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return JObject.Load(reader).ToObject<WebProxy>(serializer);
        }

        public override void WriteJson(JsonWriter writer, IWebProxy value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
