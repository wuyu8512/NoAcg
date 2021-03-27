using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NoAcgNew.Onebot.Models.EventArgs.MetaEventArgs
{
    /// <summary>
    /// 心跳包
    /// </summary>
    public sealed class HeartBeatEventArgs : BaseMetaEventArgs
    {
        /// <summary>
        /// 状态信息
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        internal JObject Status { get; set; }

        /// <summary>
        /// 到下次心跳的间隔，单位毫秒
        /// </summary>
        [JsonProperty(PropertyName = "interval")]
        internal long Interval { get; set; }
    }
}