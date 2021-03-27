using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickOperation.RequestQuickOperation
{
    public class BaseRequestQuickOperation : BaseQuickOperation
    {
        [JsonProperty(PropertyName = "approve")]
        public bool Approve { get; set; }
    }
}