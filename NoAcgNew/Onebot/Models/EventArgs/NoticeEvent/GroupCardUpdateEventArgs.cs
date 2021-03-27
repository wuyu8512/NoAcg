using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// Go扩展事件
    /// 群成员名片变更事件
    /// </summary>
    public sealed class GroupCardUpdateEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 新名片
        /// </summary>
        [JsonProperty(PropertyName = "card_new")]
        public string NewCard { get; internal init; }

        /// <summary>
        /// 旧名片
        /// </summary>
        [JsonProperty(PropertyName = "card_old")]
        public string OldCard { get; internal init; }
    }
}