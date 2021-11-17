using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class PrivateMsgQuickOperation : BaseMsgQuickOperation
    {
        public static implicit operator PrivateMsgQuickOperation(CQCode code) => new() {Reply = new[] {code}};

        public PrivateMsgQuickOperation(BaseMsgQuickOperation baseMsg)
        {
            Reply = baseMsg.Reply;
            AutoEscape = baseMsg.AutoEscape;
        }

        public PrivateMsgQuickOperation()
        {
        }
    }
}