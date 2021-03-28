using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 戳一戳事件
    /// </summary>
    public class PokeEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 被戳者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "target_id")]
        public long TargetId { get; internal init; }
        
        /// <summary>
        /// 被戳者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "sender_id")]
        public long SendId { get; internal init; }
    }
}