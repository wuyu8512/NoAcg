using System;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MsgTypeAttribute : Attribute
    {
        public CQCodeType[] MsgType { get; }

        public MsgTypeAttribute(CQCodeType type)
        {
            this.MsgType = new[] { type };
        }

        public MsgTypeAttribute(CQCodeType[] types)
        {
            if (types != null) this.MsgType = types;
        }
    }
}
