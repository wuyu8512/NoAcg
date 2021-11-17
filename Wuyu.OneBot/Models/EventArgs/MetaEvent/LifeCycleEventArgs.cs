using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;

namespace Wuyu.OneBot.Models.EventArgs.MetaEvent
{
    /// <summary>
    /// 生命周期事件
    /// </summary>
    [EventType("meta_event", "lifecycle")]
    public sealed class LifeCycleEventArgs : BaseMetaEventArgs
    {
        /// <summary>
        /// <para>事件子类型</para>
        /// <para>当前版本只可能为<see langword="connect"/></para>
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; internal init; }
    }
}
