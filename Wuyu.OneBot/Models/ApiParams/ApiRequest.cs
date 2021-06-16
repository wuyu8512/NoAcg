using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.ApiType;

namespace Wuyu.OneBot.Models.ApiParams
{
    public class ApiRequest
    {
        /// <summary>
        /// API请求类型
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        [Newtonsoft.Json.JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonPropertyName("action")]
        [System.Text.Json.Serialization.JsonConverter(typeof(Wuyu.OneBot.Converter.System.Text.Json.JsonDescriptionEnumConverter))]
        internal ApiRequestType ApiRequestType { get; set; }

        /// <summary>
        /// 请求标识符
        /// 会自动生成初始值不需要设置
        /// </summary>
        [JsonProperty(PropertyName = "echo")]
        [JsonPropertyName("echo")]
        internal Guid Echo { get; set; } = Guid.NewGuid();
    }
    
    public sealed class ApiRequest<T> : ApiRequest
    {
        /// <summary>
        /// API参数对象
        /// 不需要请使用非泛型版本
        /// </summary>
        [JsonProperty(PropertyName = "params",NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("params")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        internal T ApiParams { get; set; }
    }
}