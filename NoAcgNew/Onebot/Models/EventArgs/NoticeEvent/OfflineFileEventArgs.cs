using Newtonsoft.Json;
using NoAcgNew.Onebot.Models.EventArgs.Info;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 离线文件事件
    /// </summary>
    public class OfflineFileEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 离线文件信息
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public OfflineFileInfo Info { get; internal init; }
    }
}