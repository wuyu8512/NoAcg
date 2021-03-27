using System.Collections.Generic;
using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Onebot.Models.ApiParams
{
    public sealed class SendMessageParams
    {
        /// <summary>
        /// 消息类型 群/私聊
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "message_type")]
        internal MessageType MessageType { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        internal long? UserId { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        internal long? GroupId { get; set; }

        /// <summary>
        /// 消息段数组
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        internal IEnumerable<CQCode> Message { get; set; }

        /// <summary>
        /// 是否忽略CQ码
        /// </summary>
        [JsonProperty(PropertyName = "auto_escape")]
        internal bool AutoEscape { get; set; }
    }
}