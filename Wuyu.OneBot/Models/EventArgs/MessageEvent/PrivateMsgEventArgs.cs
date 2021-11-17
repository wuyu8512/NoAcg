using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Models.EventArgs.MessageEvent
{
    /// <summary>
    /// 私聊消息事件
    /// </summary>
    [EventType("message", "private")]
    public sealed class PrivateMsgEventArgs : BaseMessageEventArgs
    {
        /// <summary>
        /// 发送人信息
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        public PrivateSenderInfo SenderInfo { get; internal init; }
    }
}