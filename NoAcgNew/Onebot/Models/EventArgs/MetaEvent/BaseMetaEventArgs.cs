using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.MetaEvent
{
	public abstract class BaseMetaEventArgs: BaseEventArgs
	{
        /// <summary>
        /// 元事件类型
        /// </summary>
        [JsonProperty(PropertyName = "meta_event_type")]
        public string MetaEventType { get; internal init; }
    }
}
