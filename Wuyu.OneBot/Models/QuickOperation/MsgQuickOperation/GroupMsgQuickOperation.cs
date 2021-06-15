using Newtonsoft.Json;
using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class GroupMsgQuickOperation : BaseMsgQuickOperation
    {
        [JsonProperty(PropertyName = "at_sender", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AtSender { get; set; }

        [JsonProperty(PropertyName = "delete", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Delete { get; set; }

        [JsonProperty(PropertyName = "kick", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Kick { get; set; }

        [JsonProperty(PropertyName = "ban", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Ban { get; set; }

        [JsonProperty(PropertyName = "ban_duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? BanDuration { get; set; }

        public static implicit operator GroupMsgQuickOperation(CQCode code) => new() {Reply = new[] {code}};

        public GroupMsgQuickOperation(BaseMsgQuickOperation baseMsg)
        {
            Reply = baseMsg.Reply;
            AutoEscape = baseMsg.AutoEscape;
            Code = baseMsg.Code;
        }

        public GroupMsgQuickOperation()
        {
        }
    }
}