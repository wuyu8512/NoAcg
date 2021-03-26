using System.Collections.Generic;
using Newtonsoft.Json;
using NoAcgNew.Entities.CQCodes;
using NoAcgNew.Onebot.Models.ApiParams;

namespace NoAcgNew.Onebot.Models.EventResult.MsgReturn
{
    public class BaseMsgReturn : BaseEventReturn
    {
        [JsonProperty(PropertyName = "reply", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<CQCode> Reply { get; set; }

        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }
    }
}