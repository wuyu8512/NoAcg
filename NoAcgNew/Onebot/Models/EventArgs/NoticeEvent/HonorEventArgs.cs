using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群成员荣誉变更事件
    /// </summary>
    public sealed class HonorEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 荣誉类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "honor_type")]
        internal HonorType HonorType { get; set; }
    }
}