using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickOperation
{
    public class BaseQuickOperation
    {
        [JsonIgnore] public int Code { get; set; } = 0;
    }
}