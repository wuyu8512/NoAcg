using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Converter;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// At某人
    /// </summary>
    [MsgType(CQCodeType.At)]
    public struct At
    {
        #region 属性

        /// <summary>
        /// At目标UID
        /// 为<see langword="null"/>时为At全体
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "qq")]
        public string Traget { get; internal set; }

        #endregion
    }
}