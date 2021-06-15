using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.QuickOperation
{
    public class BaseQuickOperation
    {
        [JsonIgnore]
        public int Code { get; set; } = 0;
    }
}