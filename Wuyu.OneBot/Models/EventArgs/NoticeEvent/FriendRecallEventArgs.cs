using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 好友消息撤回
    /// </summary>
    [EventType("notice", "friend_recall")]
    public sealed class FriendRecallEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 被撤回的消息 ID
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public int MessageId { get; internal init; }
    }
}