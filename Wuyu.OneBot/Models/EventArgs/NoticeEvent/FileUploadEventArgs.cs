using Newtonsoft.Json;
using Wuyu.OneBot.Models.EventArgs.Info;

namespace Wuyu.OneBot.Models.EventArgs.NoticeEvent
{
    /// <summary>
    /// 群文件上传事件
    /// </summary>
    public sealed class FileUploadEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 上传的文件信息
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public UploadFileInfo Upload { get; internal init; }
    }
}