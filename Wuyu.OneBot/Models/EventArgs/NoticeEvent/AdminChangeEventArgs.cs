using Newtonsoft.Json;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群管理员变动事件
    /// </summary>
    public sealed class AdminChangeEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        public AdminChangeType SubType { get; internal init; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }
    }
}