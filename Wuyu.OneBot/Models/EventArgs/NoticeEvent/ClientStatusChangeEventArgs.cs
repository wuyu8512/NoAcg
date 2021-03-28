using Newtonsoft.Json;
using Wuyu.OneBot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 其他客户端在线状态变更
    /// </summary>
    public sealed class ClientStatusChangeEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 客户端信息
        /// </summary>
        [JsonProperty(PropertyName = "client")]
        public ClientInfo ClientInfo { get; internal init; }

        /// <summary>
        /// 是否在线
        /// </summary>
        [JsonProperty(PropertyName = "online")]
        public bool Online { get; internal init; }
    }
}