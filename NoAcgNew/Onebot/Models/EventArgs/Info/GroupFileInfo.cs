using System;
using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.Info
{
    /// <summary>
    /// 群文件信息
    /// </summary>
    public struct GroupFileInfo
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [JsonProperty(PropertyName = "file_id")]
        public string Id { get; internal init; }

        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "file_name")]
        public string Name { get; internal init; }

        /// <summary>
        /// 文件类型ID
        /// </summary>
        [JsonProperty(PropertyName = "busid")]
        public int BusId { get; internal init; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public long Size { get; internal init; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [JsonProperty(PropertyName = "upload_time")]
        public DateTime UploadTime { get; private set; }

        /// <summary>
        /// <para>过期时间</para>
        /// <para>永久文件为0</para>
        /// </summary>
        [JsonProperty(PropertyName = "dead_time")]
        public DateTime DeadTime { get; private set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonProperty(PropertyName = "modify_time")]
        public DateTime ModifyTime { get; internal init; }

        /// <summary>
        /// 下载次数
        /// </summary>
        [JsonProperty(PropertyName = "download_times")]
        public int DownloadCount { get; internal init; }

        /// <summary>
        /// 上传者UID
        /// </summary>
        [JsonProperty(PropertyName = "uploader")]
        public long UploadUserId { get; internal init; }

        /// <summary>
        /// 上传者名
        /// </summary>
        [JsonProperty(PropertyName = "uploader_name")]
        public string UploadUserName { get; internal init; }
    }
}