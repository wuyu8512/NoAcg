using System.ComponentModel;

namespace Wuyu.OneBot.Enumeration.ApiType
{
    /// <summary>
    /// API返回值
    /// </summary>
    [DefaultValue(Ok)]
    public enum ApiStatusType
    {
        /// <summary>
        /// API执行成功
        /// </summary>
        Ok = 0,

        /// <summary>
        /// API执行失败
        /// </summary>
        Failed = 100,

        /// <summary>
        /// 404
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// API执行发生内部错误
        /// </summary>
        Error = 502,

        /// <summary>
        /// API超时
        /// </summary>
        TimeOut = -1,
        
        /// <summary>
        /// 取消发送
        /// </summary>
        Cancel = -2
    }
}