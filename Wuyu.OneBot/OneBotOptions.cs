using System;
using Microsoft.Extensions.Configuration;

namespace Wuyu.OneBot
{
    public class OneBotOptions
    {
        public bool EnableWebSocketService { get; set; }
        public bool EnableWebSocketClient { get; set; }
        public string WebSocketServiceUrl { get; set; } = "/Universal";
        public string WebSocketClientUrl { get; set; } = "ws://172.18.0.254:6700";
        public bool EnableHttpPost { get; set; }
        public IConfigurationSection HttpApi { get; set; }
    }
}