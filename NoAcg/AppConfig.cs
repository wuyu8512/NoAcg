using Sora.Server;
using System.Net;

namespace NoAcg
{
    public class AppConfig
    {
        public YandeConfig YandeConfig { get; set; }
        public ServerConfig ServerConfig { get; set; }

        public static AppConfig Default = new AppConfig
        {
            ServerConfig = new ServerConfig { Port = 9200, Location = "0.0.0.0", ApiTimeOut = 3000 },
            YandeConfig = new YandeConfig { Proxy = new WebProxy("127.0.0.1:8889"), Timeout = 10000 }
        };
    }

    public class YandeConfig
    {
        public WebProxy Proxy { get; set; }
        public int Timeout { get; set; }
    }
}