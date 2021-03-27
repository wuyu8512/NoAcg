using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickOperation.RequestQuickOperation
{
    public class FriendRequestQuickOperation : BaseRequestQuickOperation
    {
        /// <summary>
        /// 添加后的好友备注 ( 仅在同意时有效 )
        /// </summary>
        [JsonProperty(PropertyName = "remark")]
        public string Remark { get; set; }

        public static implicit operator FriendRequestQuickOperation(int code) => new() {Code = code};
    }
}