using Newtonsoft.Json;
using NoAcgNew.Onebot.Models.EventArgs.Info;

namespace NoAcgNew.Onebot.Models.EventArgs.MessageEvent
{
    /// <summary>
    /// 私聊消息事件
    /// </summary>
    public sealed class PrivateMsgEventArgs : BaseMessageEventArgs
    {
        /// <summary>
        /// 发送人信息
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        internal PrivateSenderInfo SenderInfo { get; set; }
    }
}