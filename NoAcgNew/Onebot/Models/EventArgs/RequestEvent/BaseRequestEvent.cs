using Newtonsoft.Json;
using NoAcgNew.Converter;
using NoAcgNew.Enumeration.EventParamsType;

namespace NoAcgNew.Onebot.Models.EventArgs.RequestEvent
{
    /// <summary>
    /// 请求事件基类
    /// </summary>
    public abstract class BaseRequestEvent : BaseEventArgs
    {
        /// <summary>
        /// 请求类型
        /// </summary>
        [JsonProperty(PropertyName = "request_type")]
        [JsonConverter(typeof(EnumDescriptionConverter))]
        public RequestType RequestType { get; internal init; }

        /// <summary>
        /// 发送请求的 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; internal init; }

        /// <summary>
        /// 验证信息
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; internal init; }

        /// <summary>
        /// 请求 flag
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public string Flag { get; internal init; }
    }
}