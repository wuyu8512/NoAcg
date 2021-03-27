using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 精华消息变动事件
    /// </summary>
    public class EssenceChangeEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; internal init; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        public long OperatorId { get; internal init; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        [JsonProperty(PropertyName = "sender_id")]
        public long SenderId { get; internal init; }

        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        public EssenceChangeType EssenceChangeType { get; internal init; }
    }
}