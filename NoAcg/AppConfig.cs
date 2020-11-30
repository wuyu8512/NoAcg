using Sora.Server;
using System.Net;

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