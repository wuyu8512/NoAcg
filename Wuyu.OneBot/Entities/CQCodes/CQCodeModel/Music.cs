using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Enumeration.EventParamsType;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 音乐分享
    /// 仅发送
    /// </summary>
    [MsgType(CQCodeType.Music)]
    public struct Music
    {
        /// <summary>
        /// 音乐分享类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "type")]
        public MusicShareType MusicType { get; internal set; }

        /// <summary>
        /// 歌曲 ID
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "id")]
        public long MusicId { get; internal set; }
    }
}