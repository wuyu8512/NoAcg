using System.ComponentModel;

namespace Wuyu.OneBot.Enumeration.EventParamsType
{
    /// <summary>
    /// 音乐分享类型
    /// </summary>
    public enum MusicShareType
    {
        /// <summary>
        /// 网易云音乐
        /// </summary>
        [Description("163")] Netease,

        /// <summary>
        /// QQ 音乐
        /// </summary>
        [Description("qq")] QQMusic
    }
}