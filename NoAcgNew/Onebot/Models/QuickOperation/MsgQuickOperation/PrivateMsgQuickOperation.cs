using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.QuickOperation.MsgQuickOperation
{
    public class PrivateMsgQuickOperation : BaseMsgQuickOperation
    {
        public static implicit operator PrivateMsgQuickOperation(int code) => new() {Code = code};
    }
}