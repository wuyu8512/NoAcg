using System;
using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.EventArgs.Info
{
    /// <summary>
    /// 群文件夹信息
    /// </summary>
    public struct GroupFolderInfo
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        [JsonProperty(PropertyName = "folder_id")]
        public string Id { get; internal init; }

        /// <summary>
        /// 文件夹名
        /// </summary>
        [JsonProperty(PropertyName = "folder_name")]
        public string Name { get; internal init; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty(PropertyName = "create_time")]
        public DateTime CreateTime { get; internal init; }

        /// <summary>
        /// 创建者UID
        /// </summary>
        [JsonProperty(PropertyName = "creator")]
        public long CreatorUserId { get; internal init; }

        /// <summary>
        /// 创建者名
        /// </summary>
        [JsonProperty(PropertyName = "creator_name")]
        public string CreatorUserName { get; internal init; }

        /// <summary>
        /// 子文件数量
        /// </summary>
        [JsonProperty(PropertyName = "total_file_count")]
        public int FileCount { get; internal init; }
    }
}