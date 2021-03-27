using Newtonsoft.Json;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 接收红包
    /// 仅支持Go
    /// </summary>
    public struct RedBag
    {
        /// <summary>
        /// 祝福语/口令
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; internal set; }
    }
}