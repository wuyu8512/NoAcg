using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.QuickOperation.RequestQuickOperation
{
    public class BaseRequestQuickOperation : BaseQuickOperation
    {
        [JsonProperty(PropertyName = "approve")]
        public bool Approve { get; set; }
    }
}