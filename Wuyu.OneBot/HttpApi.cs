using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Enumeration.ApiType;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.ApiParams;

namespace Wuyu.OneBot
{
    public class HttpApi : IOneBotApi
    {
        private readonly IOneBotHttpApi _api;
        private readonly ILogger<HttpApi> _logger;

        public HttpApi(IOneBotHttpApi api, ILogger<HttpApi> logger)
        {
            _api = api;
            _logger = logger;
        }

        public OneBotApiType GetApiType()
        {
            return OneBotApiType.Http;
        }

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long? groupId,
            IEnumerable<CQCode> message, bool? autoEscape = default,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(userId, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(null, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendMsg(long? userId, long? groupId, IEnumerable<CQCode> message,
            bool? autoEscape = default,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SendMsg] User：{UserId} Group：{GroupId}", userId, groupId);
            var reply = await _api.SendMsg(new SendMessageParams
            {
                UserId = userId,
                GroupId = groupId,
                Message = message,
                AutoEscape = autoEscape,
            }, cancellationToken);

            var id = -1;
            if (reply?["data"] is JObject data && data.ContainsKey("message_id"))
            {
                id = data["message_id"]?.ToObject<int>() ?? -1;
            }

            // TODO ApiStatusType解析
            return reply == null ? (ApiStatusType.Error, 0) : (ApiStatusType.Ok, id);
        }
    }
}