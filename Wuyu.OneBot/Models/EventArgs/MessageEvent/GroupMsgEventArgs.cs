using Newtonsoft.Json;
using Wuyu.OneBot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Models.EventArgs.MessageEvent
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
        public long GroupId { get; internal init; }

        /// <summary>
        /// 匿名信息
        /// </summary>
        [JsonProperty(PropertyName = "anonymous", NullValueHandling = NullValueHandling.Ignore)]
        public Anonymous Anonymous { get; internal init; }

        /// <summary>
        /// 发送人信息
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        public GroupSenderInfo SenderInfo { get; internal init; }

        /// <summary>
        /// 消息序号
        /// </summary>
        [JsonProperty(PropertyName = "message_seq")]
        public int MessageSequence { get; internal init; }
    }
}