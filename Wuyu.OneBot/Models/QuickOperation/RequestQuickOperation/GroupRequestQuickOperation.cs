using Newtonsoft.Json;

namespace Wuyu.OneBot.Onebot.Models.QuickOperation.RequestQuickOperation
{
    public class GroupRequestQuickOperation : BaseRequestQuickOperation
    {
        /// <summary>
        /// 拒绝理由 ( 仅在拒绝时有效 )
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        public static implicit operator GroupRequestQuickOperation(int code) => new() {Code = code};
    }
}