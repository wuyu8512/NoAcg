using Sora.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NoAcg
{
	public class AppConfig
	{
		public YandeConfig YandeConfig { get; set; }
		public ServerConfig ServerConfig { get; set; }
	}

	public class YandeConfig
	{
		public WebProxy Proxy { get; set; }
		public int Timeout { get; set; }
	}
}
