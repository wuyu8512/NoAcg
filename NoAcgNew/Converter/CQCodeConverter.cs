using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoAcgNew.Onebot.Models;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Entities.CQCodes.CQCodeModel;
using NoAcgNew.Enumeration;

namespace NoAcgNew.Converter
{
    public class CQCodeConverter: JsonConverter<CQCode>
    {
        public override void WriteJson(JsonWriter writer, CQCode value, JsonSerializer serializer)
        {
            // writer.WriteRawValue(JsonConvert.SerializeObject(value.Data));
        }

        public override CQCode ReadJson(JsonReader reader, Type objectType, CQCode existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var messageElement = json.ToObject<MessageElement>();
            return messageElement.MsgType switch
            {
                CQCodeType.Text => new CQCode(CQCodeType.Text, messageElement.RawData.ToObject<Text>()),
                CQCodeType.Face => new CQCode(CQCodeType.Face, messageElement.RawData.ToObject<Face>()),
                CQCodeType.Image => new CQCode(CQCodeType.Image, messageElement.RawData.ToObject<Image>()),
                CQCodeType.Record => new CQCode(CQCodeType.Record, messageElement.RawData.ToObject<Record>()),
                CQCodeType.At => new CQCode(CQCodeType.At, messageElement.RawData.ToObject<At>()),
                CQCodeType.Share => new CQCode(CQCodeType.Share, messageElement.RawData.ToObject<Share>()),
                CQCodeType.Reply => new CQCode(CQCodeType.Reply, messageElement.RawData.ToObject<Reply>()),
                CQCodeType.Forward => new CQCode(CQCodeType.Forward, messageElement.RawData.ToObject<Forward>()),
                CQCodeType.Xml => new CQCode(CQCodeType.Xml, messageElement.RawData.ToObject<Code>()),
                CQCodeType.Json => new CQCode(CQCodeType.Json, messageElement.RawData.ToObject<Code>()),
                CQCodeType.Video => new CQCode(CQCodeType.Video,messageElement.RawData.ToObject<Video>()),
                CQCodeType.Gift => new CQCode(CQCodeType.Gift,messageElement.RawData.ToObject<Gift>()),
                CQCodeType.Music => new CQCode(CQCodeType.Music,messageElement.RawData.ToObject<Music>()),
                CQCodeType.Poke => new CQCode(CQCodeType.Poke,messageElement.RawData.ToObject<Poke>()),
                CQCodeType.CardImage => new CQCode(CQCodeType.CardImage,messageElement.RawData.ToObject<CardImage>()),
                CQCodeType.RedBag => new CQCode(CQCodeType.RedBag,messageElement.RawData.ToObject<RedBag>()),
                _ => new CQCode(CQCodeType.Unknown, messageElement.RawData)
            };
        }

        public override bool CanWrite { get; } = false;
    }
}