using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 短视频
    /// </summary>
    [MsgType(CQCodeType.Video)]
    public struct Video
    {
        #region 属性

        /// <summary>
        /// 视频地址, 支持http和file发送
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public string VideoUrl { get; internal set; }

        /// <summary>
        /// 视频封面, 支持http, file和base64发送, 格式必须为jpg
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "cover", NullValueHandling = NullValueHandling.Ignore)]
        public string Cover { get; internal set; }
        
        /// <summary>
        /// 是否使用已缓存的文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "cache", NullValueHandling = NullValueHandling.Ignore)]
        public int? Cache { get; internal set; }

        /// <summary>
        /// 只在通过网络 URL 发送时有效, 表示是否通过代理下载文件 ( 需通过环境变量或配置文件配置代理 ) , 默认 1
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "proxy", NullValueHandling = NullValueHandling.Ignore)]
        public int? Proxy { get; internal set; }

        /// <summary>
        /// 只在通过网络 URL 发送时有效, 单位秒, 表示下载网络文件的超时时间 , 默认不超时
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int? Timeout { get; internal set; }

        #endregion
    }
}