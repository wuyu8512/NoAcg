using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoAcg
{
	public class InvokeConfig
	{
		public List<InvokeItem> GroupConfig { get; set; }

		public List<InvokeItem> PrivateConfig { get; set; }
	}

	public class InvokeItem
	{
		public string MatchMode { get; set; } = "FullText";
		public bool Intercept { get; set; } = true;
		public string Method { get; set; }
		public string ClassName { get; set; }
		public string Text { get; set; }
		public object[] Param { get; set; }
	}
}
