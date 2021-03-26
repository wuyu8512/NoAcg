using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickAction.MsgAction
{
    public class PrivateMsgAction : BaseMsgAction
    {
        [JsonProperty(PropertyName = "at_sender")]
        public bool AtSender { get; set; } = true;

        [JsonProperty(PropertyName = "delete")]
        public bool Delete { get; set; }

        [JsonProperty(PropertyName = "kick")] public bool Kick { get; set; }

        [JsonProperty(PropertyName = "ban")] public bool Ban { get; set; }
        
        [JsonProperty(PropertyName = "ban_duration")]
        public long BanDuration { get; set; }
    }
}