using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.QuickOperation.RequestQuickOperation
{
    public class GroupRequestQuickOperation : BaseRequestQuickOperation
    {
        /// <summary>
        /// 拒绝理由 ( 仅在拒绝时有效 )
        /// </summary>
        [JsonProperty(PropertyName = "reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }
    }
}