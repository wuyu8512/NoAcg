using Wuyu.OneBot.Attributes;

namespace Wuyu.OneBot.Models.EventArgs.RequestEvent
{
    /// <summary>
    /// 好友邀请事件
    /// </summary>
    [EventType("request", "friend")]
    public sealed class FriendRequestEventArgs : BaseRequestEvent
    {
        //暂无独有字段，仅用于占位
    }
}