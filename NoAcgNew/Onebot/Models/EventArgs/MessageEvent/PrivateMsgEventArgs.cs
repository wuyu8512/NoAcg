using Newtonsoft.Json;
using NoAcgNew.Onebot.Models.Info;

namespace Sora.OnebotModel.OnebotEvent.MessageEvent
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