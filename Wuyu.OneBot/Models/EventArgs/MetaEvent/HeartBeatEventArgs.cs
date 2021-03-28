using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wuyu.OneBot.Models.EventArgs.MetaEvent
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
        public JObject Status { get; internal init; }

        /// <summary>
        /// 到下次心跳的间隔，单位毫秒
        /// </summary>
        [JsonProperty(PropertyName = "interval")]
        public long Interval { get; internal init; }
    }
}