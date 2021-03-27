using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
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
        public MuteActionType ActionType { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// 操作者 UID
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        public long OperatorId { get; set; }

        /// <summary>
        /// 禁言时长(s)
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; set; }
    }
}