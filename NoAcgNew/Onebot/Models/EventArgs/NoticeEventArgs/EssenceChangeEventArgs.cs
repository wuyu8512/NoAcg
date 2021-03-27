using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
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
        internal long GroupId { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        internal long MessageId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        internal long OperatorId { get; set; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        [JsonProperty(PropertyName = "sender_id")]
        internal long SenderId { get; set; }

        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        internal EssenceChangeType EssenceChangeType { get; set; }
    }
}