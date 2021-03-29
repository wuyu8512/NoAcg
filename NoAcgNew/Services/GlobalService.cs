using System.Net;
using Microsoft.Extensions.Configuration;

namespace NoAcgNew.Services
{
    public class GlobalService
    {
        private readonly IConfiguration _configuration;

        public GlobalService(IConfiguration configuration)
        {
            _configuration = configuration;
            WebProxy = new WebProxy(configuration["Proxy"]);
        }
        
        public WebProxy WebProxy { get; }
    }
}

