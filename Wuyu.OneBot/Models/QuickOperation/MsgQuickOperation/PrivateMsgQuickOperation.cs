using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class PrivateMsgQuickOperation : BaseMsgQuickOperation
    {
        public static implicit operator PrivateMsgQuickOperation(CQCode code) => new() {Reply = new[] {code}};

        public static implicit operator PrivateMsgQuickOperation((int code, CQCode msg) data) =>
            new() {Reply = new[] {data.msg}, Code = data.code};

        public PrivateMsgQuickOperation(BaseMsgQuickOperation baseMsg)
        {
            Code = baseMsg.Code;
            Reply = baseMsg.Reply;
            AutoEscape = baseMsg.AutoEscape;
        }

        public PrivateMsgQuickOperation()
        {
        }
    }
}