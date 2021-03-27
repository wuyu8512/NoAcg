using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.RequestEvent
{
    /// <summary>
    /// 群聊邀请/入群请求事件
    /// </summary>
    public sealed class GroupRequestEventArgs : BaseRequestEvent
    {
        /// <summary>
        /// 请求子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        public GroupRequestType GroupRequestType { get; internal init; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }
    }
}