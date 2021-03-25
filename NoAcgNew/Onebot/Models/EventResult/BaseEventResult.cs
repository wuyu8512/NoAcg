using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.ApiParams
{
    public class BaseEventReturn
    {
        [JsonIgnore] public int Code { get; set; } = 0;
    }
}