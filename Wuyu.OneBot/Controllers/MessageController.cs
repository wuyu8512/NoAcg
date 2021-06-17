using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wuyu.OneBot.Models.QuickOperation;

namespace Wuyu.OneBot.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly EventManager _eventManager;
        private readonly HttpApi _api;

        public MessageController(ILogger<MessageController> logger, EventManager eventManager, HttpApi httpApi)
        {
            _logger = logger;
            _eventManager = eventManager;
            _api = httpApi;
        }

        [HttpPost]
        public async ValueTask<IActionResult> Post()
        {
            if (!Main.Options.EnableHttpPost) return Ok();
            string rawMsg;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                rawMsg = await reader.ReadToEndAsync();
            }

            var json = JObject.Parse(rawMsg);
            if (!json.ContainsKey("post_type")) return Ok();
            var result = await _eventManager.Adapter(json, _api, rawMsg);
            if (result is BaseQuickOperation reply)
            {
                _logger.LogInformation("[HandleQuickOperation]返回快速操作");
                return Ok(JsonConvert.SerializeObject(reply, Formatting.None));
            }

            return Ok();
        }
    }
}