using System;

namespace Wuyu.OneBot.Models.EventArgs.Info
{
    /// <summary>
    /// 精华消息信息
    /// </summary>
    public readonly struct EssenceInfo
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public long MessageId { get;internal init; }

        /// <summary>
        /// 精华设置者
        /// </summary>
        public long Operator { get;internal init; }

        /// <summary>
        /// 精华设置者用户名
        /// </summary>
        public string OperatorName { get;internal init; }

        /// <summary>
        /// 精华设置时间
        /// </summary>
        public DateTime Time { get;internal init; }

        /// <summary>
        /// 消息发送者
        /// </summary>
        public long Sender { get;internal init; }

        /// <summary>
        /// 消息发送者名
        /// </summary>
        public string SenderName { get; internal init;}

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime MessageSendTime { get;internal init; }
    }
}