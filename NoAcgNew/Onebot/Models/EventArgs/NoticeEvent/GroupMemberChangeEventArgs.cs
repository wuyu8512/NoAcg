using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群成员变动事件
    /// </summary>
    public sealed class GroupMemberChangeEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        public MemberChangeType SubType { get; internal init; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 操作者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        public long OperatorId { get; internal init; }
    }
}