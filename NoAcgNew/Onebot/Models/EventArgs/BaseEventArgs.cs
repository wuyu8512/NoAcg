using NoAcgNew.Interfaces;
using NoAcgNew.Expansion;
using System;
using Newtonsoft.Json;
using Sora.Converter;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 框架事件基类
    /// </summary>
    public abstract class BaseEventArgs : System.EventArgs
    {
        /// <summary>
        /// 事件发生的时间
        /// </summary>
        [JsonProperty(PropertyName = "time", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DataTimeConverter))]
        internal DateTime Time { get; set; }

        /// <summary>
        /// 收到事件的机器人 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "self_id", NullValueHandling = NullValueHandling.Ignore)]
        internal long SelfID { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonProperty(PropertyName = "post_type", NullValueHandling = NullValueHandling.Ignore)]
        internal string PostType { get; set; }
    }
}