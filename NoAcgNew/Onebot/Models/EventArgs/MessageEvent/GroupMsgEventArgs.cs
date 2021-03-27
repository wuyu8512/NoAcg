using Newtonsoft.Json;
using NoAcgNew.Onebot.Models.EventArgs.Info;

namespace NoAcgNew.Onebot.Models.EventArgs.MessageEvent
{
    /// <summary>
    /// 群组消息事件
    /// </summary>
    public sealed class GroupMsgEventArgs: BaseMessageEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 匿名信息
        /// </summary>
        [JsonProperty(PropertyName = "anonymous", NullValueHandling = NullValueHandling.Ignore)]
        internal Anonymous Anonymous { get; set; }

        /// <summary>
        /// 发送人信息
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        internal GroupSenderInfo SenderInfo { get; set; }

        /// <summary>
        /// 消息序号
        /// </summary>
        [JsonProperty(PropertyName = "message_seq")]
        internal int MessageSequence { get; set; }
    }
}