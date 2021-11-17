using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Models.EventArgs.RequestEvent
{
    /// <summary>
    /// 群聊邀请/入群请求事件
    /// </summary>
    [EventType("request", "group")]
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