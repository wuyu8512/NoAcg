using Newtonsoft.Json;

namespace Wuyu.OneBot.Onebot.Models.QuickOperation.RequestQuickOperation
{
    public class BaseRequestQuickOperation : BaseQuickOperation
    {
        [JsonProperty(PropertyName = "approve")]
        public bool Approve { get; set; }
    }
}