using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 好友消息撤回
    /// </summary>
    public sealed class FriendRecallEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 被撤回的消息 ID
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public int MessageId { get; internal init; }
    }
}