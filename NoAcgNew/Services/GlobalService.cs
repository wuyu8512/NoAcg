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
            WebProxy = new WebProxy(configuration["Proxy"]);
            YandeSetting.HotImg = configuration.GetSection("Yande").GetSection("HotImg")
                .Get<YandeSetting.HotImgSetting>();
            YandeSetting.CustomTags = configuration.GetSection("Yande").GetSection("CustomTags")
                .Get<YandeSetting.CustomTagsSetting[]>();
        }

        public WebProxy WebProxy { get; }
        public YandeSetting YandeSetting { get; } = new();
    }
}