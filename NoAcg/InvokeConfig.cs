using System.Collections.Generic;

namespace NoAcg
{
    public class InvokeConfig
    {
        public long[] BlackLists { get; set; }
        public InvokeItem[] UniversalConfigs { get; set; } = System.Array.Empty<InvokeItem>();
        public InvokeItem[] GroupConfigs { get; set; } = System.Array.Empty<InvokeItem>();
        public InvokeItem[] PrivateConfigs { get; set; } = System.Array.Empty<InvokeItem>();

        public static InvokeConfig Default = new InvokeConfig
        {
            GroupConfigs = new[]
                    {
                        new InvokeItem()
                        {
                            Text = "热门图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetHotImg",
                            Param = new object[] {3, true, true}
                        },
                        new InvokeItem()
                        {
                            Text = "热门Cos",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetCosHot",
                        },
                        new InvokeItem()
                        {
                            Text = "随机Cos",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetRandCos"
                        },
                        new InvokeItem()
                        {
                            Text = "随机图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetRandom"
                        }
                    },
            PrivateConfigs = new[]
                    {
                        new InvokeItem()
                        {
                            Text = "热门图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetHotImg",
                            Param = new object[] {7, true, false}
                        },
                        new InvokeItem()
                        {
                            Text = "随机奶子",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetImgByTag",
                            Param = new object[] {"breasts", 1, true, true}
                        },
                        new InvokeItem()
                        {
                            Text = @"奶子(\d)连",
                            MatchMode = "Regex",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetImgByTag",
                            Param = new object[] {"breasts", @"int:$1", true, true}
                        },
                        new InvokeItem()
                        {
                            Text = "刷新配置",
                            ClassName = "NoAcg.Core.Config",
                            Method = "RefreshInvokeConfig"
                        }
                    }
        };
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