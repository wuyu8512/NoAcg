using Wuyu.OneBot.Attributes;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 好友添加事件
    /// </summary>
    [EventType("notice", "friend")]
    public sealed class FriendAddEventArgs : BaseNoticeEventArgs
    {
        //暂无独有字段，仅用于占位
    }
}