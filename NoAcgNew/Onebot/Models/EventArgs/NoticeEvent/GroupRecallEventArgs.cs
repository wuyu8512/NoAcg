using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群消息撤回事件
    /// </summary>
    public sealed class GroupRecallEventArgs : BaseNoticeEventArgs
    {
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

        /// <summary>
        /// 被撤回的消息 ID
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public int MessageId { get; internal init; }
    }
}