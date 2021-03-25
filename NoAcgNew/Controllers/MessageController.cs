using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NoAcgNew.Onebot.Event;
using NoAcgNew.Onebot.Models.ApiParams;
using NoAcgNew.Service;

namespace NoAcgNew.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private EventManager _eventManager;
        private HttpApi _api;

        public MessageController(ILogger<MessageController> logger, EventManager eventManager, HttpApi httpApi)
        {
            _logger = logger;
            _eventManager = eventManager;
            _api = httpApi;
        }

        [HttpPost]
        public async ValueTask<ActionResult> Post()
        {
            string rawMsg;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                rawMsg = await reader.ReadToEndAsync();
            }
            var json = JObject.Parse(rawMsg);
            if (!json.ContainsKey("post_type")) return Ok();
            var result = await _eventManager.Adapter(json, _api, rawMsg);
            return result switch
            {
                null => Ok(),
                BaseEventReturn replay => Ok(replay),
                _ => Ok()
            };
        }
    }
}