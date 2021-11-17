using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群成员荣誉变更事件
    /// </summary>
    [EventType("notice", "notify", "honor")]
    public sealed class HonorEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 荣誉类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "honor_type")]
        public HonorType HonorType { get; internal init; }
    }
}