using System;

namespace Wuyu.OneBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class EventTypeAttribute : Attribute
    {
        public string PostType { get; }
        public string[] Type { get; }
        public string SubType { get; }

        public EventTypeAttribute(string postType, string type = null, string subType = "allType")
        {
            this.PostType = postType;
            if(type != null) this.Type = new string[] { type };
            this.SubType = subType;
        }

        public EventTypeAttribute(string postType, string[] type = null, string subType = "allType")
        {
            this.PostType = postType;
            if (type != null) this.Type = type;
            this.SubType = subType;
        }
    }
}
