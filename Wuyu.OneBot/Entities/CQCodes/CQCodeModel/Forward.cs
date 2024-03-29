using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 合并转发/合并转发节点
    /// </summary>
    [MsgType(CQCodeType.Forward)]
    public struct Forward
    {
        #region 属性

        /// <summary>
        /// 转发消息ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string MessageId { get; internal set; }

        #endregion
    }
}