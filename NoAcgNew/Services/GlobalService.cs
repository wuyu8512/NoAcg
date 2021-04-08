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
            WebProxy = configuration.GetSection("Proxy").Get<WebProxy>();
            YandeSetting.HotImg = configuration.GetSection("Yande").GetSection("HotImg")
                .Get<YandeSetting.HotImgSetting>();
            YandeSetting.CustomTags = configuration.GetSection("Yande").GetSection("CustomTags")
                .Get<YandeSetting.CustomTagsSetting[]>();

            TwitterSetting.Monitor = configuration.GetSection("Twitter").GetSection("Monitor")
                .Get<TwitterSetting.MonitorSetting[]>().ToDictionary(m => m.Name);
        }

        public WebProxy WebProxy { get; }
        public YandeSetting YandeSetting { get; } = new();
        public TwitterSetting TwitterSetting { get; } = new();
    }
}