using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoAcgNew.Onebot.Models;
using Sora.Entities.CQCodes;
using Sora.Entities.CQCodes.CQCodeModel;
using Sora.Enumeration;

namespace Sora.Converter
{
    public class CQCodeConverter: JsonConverter<CQCode>
    {
        public override void WriteJson(JsonWriter writer, CQCode value, JsonSerializer serializer)
        {
            writer.WriteValue(JsonConvert.SerializeObject(value));
        }

        public override CQCode ReadJson(JsonReader reader, Type objectType, CQCode existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var messageElement = json.ToObject<MessageElement>();
            return messageElement.MsgType switch
            {
                CQFunction.Text => new CQCode(CQFunction.Text, messageElement.RawData.ToObject<Text>()),
                CQFunction.Face => new CQCode(CQFunction.Face, messageElement.RawData.ToObject<Face>()),
                CQFunction.Image => new CQCode(CQFunction.Image, messageElement.RawData.ToObject<Image>()),
                CQFunction.Record => new CQCode(CQFunction.Record, messageElement.RawData.ToObject<Record>()),
                CQFunction.At => new CQCode(CQFunction.At, messageElement.RawData.ToObject<At>()),
                CQFunction.Share => new CQCode(CQFunction.Share, messageElement.RawData.ToObject<Share>()),
                CQFunction.Reply => new CQCode(CQFunction.Reply, messageElement.RawData.ToObject<Reply>()),
                CQFunction.Forward => new CQCode(CQFunction.Forward, messageElement.RawData.ToObject<Forward>()),
                CQFunction.Xml => new CQCode(CQFunction.Xml, messageElement.RawData.ToObject<Code>()),
                CQFunction.Json => new CQCode(CQFunction.Json, messageElement.RawData.ToObject<Code>()),
                _ => new CQCode(CQFunction.Unknown, messageElement.RawData)
            };
        }
    }
}