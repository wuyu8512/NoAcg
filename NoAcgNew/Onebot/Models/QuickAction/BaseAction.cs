using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickAction
{
    public class BaseAction
    {
        [JsonIgnore] public int Code { get; set; } = 0;
    }
}