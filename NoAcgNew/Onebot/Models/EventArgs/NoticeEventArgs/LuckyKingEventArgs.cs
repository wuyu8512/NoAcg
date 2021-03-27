using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
{
    public class LuckyKingEventArgs
    {
        /// <summary>
        /// 群红包运气王事件
        /// </summary>
        internal sealed class GroupPokeEventArgs : BaseNotifyEventArgs
        {
            /// <summary>
            /// 运气王UID
            /// </summary>
            [JsonProperty(PropertyName = "target_id")]
            internal long TargetId { get; set; }
        }
    }
}