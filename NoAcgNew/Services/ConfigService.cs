using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoAcgNew.Setting;

namespace NoAcgNew.Services
{
    public class ConfigService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigService> _logger;
        public event Action<ConfigService> OnLoad;

        public ConfigService(IConfiguration configuration,ILogger<ConfigService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            Load();
        }

        private void ReLoad()
        {
            _logger.LogInformation("正在加载配置文件");

            YandeSetting.HotImg = _configuration.GetSection("Yande").GetSection("HotImg")
                .Get<YandeSetting.HotImgSetting>();
            YandeSetting.CustomTags = _configuration.GetSection("Yande").GetSection("CustomTags")
                .Get<YandeSetting.CustomTagsSetting[]>();

            TwitterSetting.Monitor = _configuration.GetSection("Twitter").GetSection("Monitor")
                .Get<TwitterSetting.MonitorSetting[]>().ToDictionary(m => m.Name);

            BiliSetting = _configuration.GetSection("BiliBili").Get<BiliSetting>();
            
            OnLoad?.Invoke(this);
        }

        private void Load()
        {            
            var token = _configuration.GetReloadToken();
            token.RegisterChangeCallback(o =>
            {
                Load();
            }, null);
            ReLoad();
        }
        
        public YandeSetting YandeSetting { get; } = new();
        public TwitterSetting TwitterSetting { get; } = new();
        public BiliSetting BiliSetting { get; set; } = new();
    }
}