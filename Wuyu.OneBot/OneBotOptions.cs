using System;
using Microsoft.Extensions.Configuration;

namespace Wuyu.OneBot
{
    public class OneBotOptions
    {
        public bool EnableWebSocketService { get; set; }
        public string WebSocketUrl { get; set; } = "/Universal";
        public bool EnableHttpPost { get; set; }
        public IConfigurationSection HttpApi { get; set; }
    }
}