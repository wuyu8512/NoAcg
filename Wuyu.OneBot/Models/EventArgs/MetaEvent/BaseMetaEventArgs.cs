using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.EventArgs.MetaEvent
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
