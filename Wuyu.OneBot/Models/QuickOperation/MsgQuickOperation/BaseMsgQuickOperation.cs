﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class BaseMsgQuickOperation : BaseQuickOperation
    {
        [JsonProperty(PropertyName = "reply", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<CQCode> Reply { get; set; }

        [JsonProperty(PropertyName = "auto_escape", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoEscape { get; set; }

        public static implicit operator BaseMsgQuickOperation(CQCode msg) => new() {Reply = new[] {msg}};

        public static implicit operator BaseMsgQuickOperation(CQCode[] msg) => new() {Reply = msg};
    }
}