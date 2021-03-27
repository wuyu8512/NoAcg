using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wuyu.OneBot.Onebot.Models
{
    /// <summary>
    /// Onebot消息段
    /// </summary>
    public sealed class MessageElement
    {
        /// <summary>
        /// 消息段类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "type")]
        internal CQCodeType MsgType { get; set; }

        /// <summary>
        /// 消息段JSON
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        internal JObject RawData { get; set; }
    }
}
