using System.ComponentModel;

namespace NoAcgNew.Enumeration.EventParamsType
{
    /// <summary>
    /// 禁言操作类型
    /// </summary>
    public enum MuteActionType
    {
        /// <summary>
        /// 开启
        /// </summary>
        [Description("ban")] Enable,

        /// <summary>
        /// 解除
        /// </summary>
        [Description("lift_ban")] Disable
    }
}