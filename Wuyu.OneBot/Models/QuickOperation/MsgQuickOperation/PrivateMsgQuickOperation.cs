namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class PrivateMsgQuickOperation : BaseMsgQuickOperation
    {
        public static implicit operator PrivateMsgQuickOperation(int code) => new() {Code = code};
        
        public PrivateMsgQuickOperation(BaseMsgQuickOperation baseMsg)
        {
            Reply = baseMsg.Reply;
            Code = baseMsg.Code;
            AutoEscape = baseMsg.AutoEscape;
        }
        
        public PrivateMsgQuickOperation()
        {
        }
    }
}