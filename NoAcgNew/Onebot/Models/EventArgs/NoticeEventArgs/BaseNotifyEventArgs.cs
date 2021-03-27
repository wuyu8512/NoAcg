using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
{
    /// <summary>
    /// 群内通知类事件
    /// </summary>
    public abstract class BaseNotifyEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal set; }
    }
}