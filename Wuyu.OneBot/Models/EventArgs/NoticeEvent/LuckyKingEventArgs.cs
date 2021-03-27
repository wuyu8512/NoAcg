using Newtonsoft.Json;

namespace Wuyu.OneBot.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群红包运气王事件
    /// </summary>
    public class LuckyKingEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 运气王UID
        /// </summary>
        [JsonProperty(PropertyName = "target_id")]
        public long TargetId { get; internal init; }
    }
}