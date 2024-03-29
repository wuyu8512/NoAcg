using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 离线文件事件
    /// </summary>
    [EventType("notice", "offline_file")]
    public class OfflineFileEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 离线文件信息
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public OfflineFileInfo Info { get; internal init; }
    }
}