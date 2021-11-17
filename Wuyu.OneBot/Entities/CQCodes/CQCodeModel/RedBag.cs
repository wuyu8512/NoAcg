using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 接收红包
    /// 仅支持Go
    /// </summary>
    [MsgType(CQCodeType.RedBag)]
    public struct RedBag
    {
        /// <summary>
        /// 祝福语/口令
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; internal set; }
    }
}