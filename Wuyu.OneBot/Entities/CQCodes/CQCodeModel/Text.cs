using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 纯文本
    /// </summary>
    [MsgType(CQCodeType.Text)]
    public struct Text
    {
        #region 属性

        /// <summary>
        /// 纯文本内容
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Content { get; internal set; }

        #endregion
    }
}