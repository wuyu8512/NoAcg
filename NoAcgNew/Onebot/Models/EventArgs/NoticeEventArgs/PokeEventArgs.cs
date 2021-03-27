using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
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
        internal long TargetId { get; set; }
        
        /// <summary>
        /// 被戳者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "sender_id")]
        internal long SendId { get; set; }
    }
}