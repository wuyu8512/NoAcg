using System.Collections.Generic;
using System.Net;

namespace NoAcg.Models
{
	public class TweeterMonitorConfig
	{
		public WebProxy Proxy { get; set; }
		public List<MonitorItem> Items { get; set; }
	}

	public class MonitorItem
	{
		public object Mark { get; set; }

		// public bool Open { get; set; }
		public List<long> Group { get; set; }

		public List<long> Private { get; set; }
	}
}