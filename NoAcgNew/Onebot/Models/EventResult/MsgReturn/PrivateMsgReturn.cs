using System.Collections.Generic;
using Newtonsoft.Json;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Onebot.Models.ApiParams;
using NoAcgNew.Onebot.Models.EventResult.MsgReturn;

namespace NoAcgNew.Onebot.Models
{
    public class PrivateMsgReturn : BaseMsgReturn
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