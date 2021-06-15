using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoAcgNew.Models;

namespace NoAcgNew.Services
{
    public class GlobalService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GlobalService> _logger;
        public event Action<GlobalService> OnLoad;

        public GlobalService(IConfiguration configuration,ILogger<GlobalService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            HttpClientProxyHandler = new HttpClientHandler {UseProxy = true};
            Load(null);
        }

        private void ReLoad()
        {
            _logger.LogInformation("正在加载配置文件");
            WebProxy = _configuration.GetSection("Proxy").Get<WebProxy>();
            YandeSetting.HotImg = _configuration.GetSection("Yande").GetSection("HotImg")
                .Get<YandeSetting.HotImgSetting>();
            YandeSetting.CustomTags = _configuration.GetSection("Yande").GetSection("CustomTags")
                .Get<YandeSetting.CustomTagsSetting[]>();

            TwitterSetting.Monitor = _configuration.GetSection("Twitter").GetSection("Monitor")
                .Get<TwitterSetting.MonitorSetting[]>().ToDictionary(m => m.Name);

            BiliSetting = _configuration.GetSection("BiliBili").Get<BiliSetting>();
            
            if (WebProxy?.Address != null)
            {
                HttpClientProxyHandler.UseProxy = true;
                HttpClientProxyHandler.Proxy = WebProxy;
            }
            else
            {
                HttpClientProxyHandler.UseProxy = false;
            }
            
            OnLoad?.Invoke(this);
        }

        private void Load(object obj)
        {            
            var token = _configuration.GetReloadToken();
            token.RegisterChangeCallback(o =>
            {
                Load(null);
            }, null);
            ReLoad();
        }
        
        public WebProxy WebProxy { get; private set; }
        public HttpClientHandler HttpClientProxyHandler { get; private set; }
        public YandeSetting YandeSetting { get; } = new();
        public TwitterSetting TwitterSetting { get; } = new();
        public BiliSetting BiliSetting { get; set; } = new();
    }
}