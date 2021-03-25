using Newtonsoft.Json;
using NoAcgNew.EventArgs.NoAcgNewEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoAcgNew.EventArgs.OneBotEventArgs.MetaEventArgs
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
