using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 消息事件基类
    /// </summary>
    public abstract class BaseNoticeEventArgs : BaseEventArgs
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonProperty(PropertyName = "notice_type", NullValueHandling = NullValueHandling.Ignore)]
        public string NoticeType { get; internal init; }

        /// <summary>
        /// 操作对象UID
        /// </summary>
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        public long UserId { get; internal init; }
    }
}