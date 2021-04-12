using System;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using NoAcgNew.Models;

namespace NoAcgNew.Services
{
    public class GlobalService
    {
        private readonly IConfiguration _configuration;

        public GlobalService(IConfiguration configuration)
        {
            _configuration = configuration;
            Load(null);
        }

        private void ReLoad()
        {
            WebProxy = _configuration.GetSection("Proxy").Get<WebProxy>();
            YandeSetting.HotImg = _configuration.GetSection("Yande").GetSection("HotImg")
                .Get<YandeSetting.HotImgSetting>();
            YandeSetting.CustomTags = _configuration.GetSection("Yande").GetSection("CustomTags")
                .Get<YandeSetting.CustomTagsSetting[]>();

            TwitterSetting.Monitor = _configuration.GetSection("Twitter").GetSection("Monitor")
                .Get<TwitterSetting.MonitorSetting[]>().ToDictionary(m => m.Name);
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
        public YandeSetting YandeSetting { get; } = new();
        public TwitterSetting TwitterSetting { get; } = new();
    }
}