using System.Collections.Generic;

namespace NoAcg
{
    public class InvokeConfig
    {
        public long[] BlackLists { get; set; }
        public InvokeItem[] UniversalConfigs { get; set; } = System.Array.Empty<InvokeItem>();
        public InvokeItem[] GroupConfigs { get; set; } = System.Array.Empty<InvokeItem>();
        public InvokeItem[] PrivateConfigs { get; set; } = System.Array.Empty<InvokeItem>();
    }

    public class InvokeItem
    {
        public string MatchMode { get; set; } = "FullText";
        public bool Intercept { get; set; } = true;
        public string Method { get; set; }
        public string ClassName { get; set; }
        public string Text { get; set; }
        public object[] Param { get; set; } = System.Array.Empty<object>();
    }
}