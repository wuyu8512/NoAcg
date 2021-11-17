using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// <para>群成员戳一戳</para>
    /// <para>仅发送</para>
    /// <para>仅支持Go</para>
    /// </summary>
    [MsgType(CQCodeType.Poke)]
    public struct Poke
    {
        #region 属性

        /// <summary>
        /// 需要戳的成员
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "qq")]
        public long Uid { get; internal set; }

        #endregion
    }
}