using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// <para>Xml与Json集合</para>
    /// <para>可能为<see cref="CQCodeType"/>.<see langword="Json"/>或<see cref="CQCodeType"/>.<see langword="Xml"/></para>
    /// </summary>
    [MsgType(new[] { CQCodeType.Xml, CQCodeType.Json })]
    public struct Code
    {
        #region 属性

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Content { get; internal set; }

        /// <summary>
        /// 是否走富文本通道
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "resid", NullValueHandling = NullValueHandling.Ignore)]
        public int? Resid { get; internal set; }

        #endregion
    }
}