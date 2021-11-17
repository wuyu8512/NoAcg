using Newtonsoft.Json;
using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.EventArgs.MessageEvent
{
    /// <summary>
    /// 消息事件基类
    /// </summary>
    public abstract class BaseMessageEventArgs : BaseEventArgs
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonProperty(PropertyName = "message_type")]
        public string MessageType { get; internal init; }

        /// <summary>
        /// 消息子类型
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        public string SubType { get; internal init; }

        /// <summary>
        /// 消息 ID
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public string MessageId { get; internal init; }

        /// <summary>
        /// 发送者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; internal init; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public CQCode[] MessageList { get; internal init; }

        /// <summary>
        /// 原始消息内容
        /// </summary>
        [JsonProperty(PropertyName = "raw_message")]
        public string RawMessage { get; internal init; }

        /// <summary>
        /// 字体
        /// </summary>
        [JsonProperty(PropertyName = "font")]
        public int Font { get; internal init; }
    }
}