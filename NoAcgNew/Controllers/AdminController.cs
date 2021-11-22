using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoAcgNew.Services;
using Wuyu.OneBot.Models.QuickOperation;

namespace NoAcgNew.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ConfigService _globalService;

        public AdminController(ILogger<AdminController> logger, ConfigService globalService)
        {
            _logger = logger;
            _globalService = globalService;
        }

        [HttpGet]
        public async ValueTask PrintConfig()
        {
            _logger.LogDebug(JsonConvert.SerializeObject(_globalService));
        }
    }
}