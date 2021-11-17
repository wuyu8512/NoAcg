using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Entities.CQCodes.CQCodeModel;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Models;

namespace Wuyu.OneBot.Converter
{
    internal class CQCodeConverter : JsonConverter<CQCode>
    {
        private readonly static List<(Type type, MsgTypeAttribute customAttributes)> msgTypes;

        static CQCodeConverter()
        {
            var assembly = Assembly.GetExecutingAssembly();
            msgTypes = new List<(Type type, MsgTypeAttribute customAttributes)>();
            assembly.GetTypes().ToList().ForEach(type =>
            {
                var customAttributes = type.GetCustomAttributes(typeof(MsgTypeAttribute), false).FirstOrDefault();
                if (customAttributes != null)
                {
                    msgTypes.Add((type, customAttributes as MsgTypeAttribute));
                }
            });
        }

        public override void WriteJson(JsonWriter writer, CQCode value, JsonSerializer serializer)
        {
            // writer.WriteRawValue(JsonConvert.SerializeObject(value.Data));
        }

        public override CQCode ReadJson(JsonReader reader, Type objectType, CQCode existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var messageElement = json.ToObject<MessageElement>();

            if (messageElement?.MsgType != null)
            {
                // 特殊处理某些Type
                if (messageElement.MsgType == CQCodeType.Music)
                {
                    if (messageElement.RawData["type"].ToString() == "custom") return new CQCode(CQCodeType.Music, messageElement.RawData.ToObject<CustomMusic>());
                    else return new CQCode(CQCodeType.Music, messageElement.RawData.ToObject<Music>());
                }
                else
                {
                    var msgType = msgTypes.FirstOrDefault(x => x.customAttributes.MsgType.Contains(messageElement.MsgType));
                    if (msgType != default)
                    {
                        return new CQCode(messageElement.MsgType, messageElement.RawData.ToObject(msgType.type));
                    }
                }
            }

            return new CQCode(CQCodeType.Unknown, messageElement?.RawData);
        }

        public override bool CanWrite { get; } = false;
    }
}