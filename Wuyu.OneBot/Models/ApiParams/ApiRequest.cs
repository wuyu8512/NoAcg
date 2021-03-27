using System;
using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.ApiType;

namespace Wuyu.OneBot.Onebot.Models.ApiParams
{
    public sealed class ApiRequest<T> : ApiRequest
    {
        /// <summary>
        /// API参数对象
        /// 不需要请使用非泛型版本
        /// </summary>
        [JsonProperty(PropertyName = "params",NullValueHandling = NullValueHandling.Ignore)]
        internal T ApiParams { get; set; }
    }
    
    public class ApiRequest
    {
        /// <summary>
        /// API请求类型
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        [JsonConverter(typeof(EnumDescriptionConverter))]
        internal ApiRequestType ApiRequestType { get; set; }

        /// <summary>
        /// 请求标识符
        /// 会自动生成初始值不需要设置
        /// </summary>
        [JsonProperty(PropertyName = "echo")]
        internal Guid Echo { get; set; } = Guid.NewGuid();
    }
}