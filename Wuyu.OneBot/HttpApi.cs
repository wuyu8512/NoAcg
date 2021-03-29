using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Enumeration.ApiType;
using Wuyu.OneBot.Enumeration.EventParamsType;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.ApiParams;

namespace Wuyu.OneBot
{
    public class HttpApi : IOneBotApi
    {
        private readonly IOneBotHttpApi _api;

        public HttpApi(IOneBotHttpApi api)
        {
            _api = api;
        }

        public OneBotApiType GetApiType()
        {
            return OneBotApiType.Http;
        }

        public async ValueTask<(ApiStatusType, int)> SendPrivateMsg(long userId, long? groupId,
            IEnumerable<CQCode> message, bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(userId, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendGroupMsg(long groupId, IEnumerable<CQCode> message,
            bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            return await SendMsg(null, groupId, message, autoEscape, cancellationToken);
        }

        public async ValueTask<(ApiStatusType, int)> SendMsg(long? userId, long? groupId, IEnumerable<CQCode> message,
            bool autoEscape = false,
            CancellationToken cancellationToken = default)
        {
            var replay = await _api.SendMsg(new SendMessageParams
            {
                UserId = userId,
                GroupId = groupId,
                Message = message,
                AutoEscape = autoEscape,
            }, cancellationToken);

            // TODO ApiStatusType解析
            return replay == null
                ? (ApiStatusType.Error, 0)
                : (ApiStatusType.Ok, replay["data"]?["message_id"]?.ToObject<int>() ?? -1);
        }
    }
}