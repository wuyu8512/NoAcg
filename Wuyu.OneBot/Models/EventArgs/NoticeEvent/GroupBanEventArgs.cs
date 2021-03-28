using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群禁言事件
    /// </summary>
    public sealed class GroupBanEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        public MuteActionType ActionType { get; internal init; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 操作者 UID
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        public long OperatorId { get; internal init; }

        /// <summary>
        /// 禁言时长(s)
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; internal init; }
    }
}