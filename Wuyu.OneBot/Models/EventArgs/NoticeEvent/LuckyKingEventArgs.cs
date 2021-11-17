using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群红包运气王事件
    /// </summary>
    [EventType("notice", "notify", "lucky_king")]
    public class LuckyKingEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 运气王UID
        /// </summary>
        [JsonProperty(PropertyName = "target_id")]
        public long TargetId { get; internal init; }
    }
}