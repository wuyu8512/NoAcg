using Newtonsoft.Json;
using Wuyu.OneBot.Onebot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Onebot.Models.EventArgs.MessageEvent
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
        public PrivateSenderInfo SenderInfo { get; internal init; }
    }
}