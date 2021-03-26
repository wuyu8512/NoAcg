using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.MetaEventArgs
{
	public abstract class BaseMetaEventArgs: BaseEventArgs
	{
        /// <summary>
        /// 元事件类型
        /// </summary>
        [JsonProperty(PropertyName = "meta_event_type")]
        internal string MetaEventType { get; set; }
    }
}
