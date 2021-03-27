﻿using System.Collections.Generic;
using Newtonsoft.Json;
using NoAcgNew.Entities.CQCodes;

namespace NoAcgNew.Onebot.Models.QuickOperation.MsgQuickOperation
{
    public class BaseMsgQuickOperation : BaseQuickOperation
    {
        [JsonProperty(PropertyName = "reply", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<CQCode> Reply { get; set; }

        [JsonProperty(PropertyName = "auto_escape")]
        public bool AutoEscape { get; set; }
    }
}