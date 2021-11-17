using Microsoft.Extensions.Logging;
using NoAcgNew.Core;
using NoAcgNew.Services;
using System;
using System.Threading.Tasks;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Expansion;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Handler
{
    public class YandeHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<YandeHandler> _logger;
        private readonly GlobalService _globalService;
        private readonly IServiceProvider _provider;
        private readonly YandeApi _yandeApi;

        public YandeHandler(EventManager eventManager,
            ILogger<YandeHandler> logger, GlobalService globalService, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _logger = logger;
            eventManager.OnGroupMessage += HandlerExpansion.ToGroupHandler(Handler);
            eventManager.OnPrivateMessage += HandlerExpansion.ToPrivateHandler(Handler);
            _globalService = globalService;
            _provider = provider;

            _yandeApi = new YandeApi(_globalService.HttpClientProxyHandler);
        }

        private async ValueTask<EventResult<BaseMsgQuickOperation>> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            long? groupId = null;
            long? userId = null;
            if (args is GroupMsgEventArgs groupArgs) groupId = groupArgs.GroupId;
            else userId = args.UserId;

            if (args.RawMessage.Equals(_globalService.YandeSetting.HotImg.Command,
                StringComparison.CurrentCultureIgnoreCase))
            {
                var (data, rating) = await _yandeApi.GetHotImgAsync(_globalService.YandeSetting.HotImg.Rating);
                var replay = CQCode.CQImage("base64://" + Convert.ToBase64String(data));
                return (1, replay);
            }

            foreach (var customTags in _globalService.YandeSetting.CustomTags)
            {
                if (args.RawMessage.Equals(customTags.Command, StringComparison.CurrentCultureIgnoreCase))
                {
                    var page = await _yandeApi.GetTagsPageAsync(customTags.Tag);
                    for (var i = 0; i < customTags.Count; i++)
                    {
                        var _ = Task.Run(async () =>
                        {
                            var (data, rating) = await _yandeApi.GetImageByTagsAsync(customTags.Tag, page, customTags.Rating);
                            await api.SendMsg(userId, groupId, new[] { CQCode.CQImage("base64://" + Convert.ToBase64String(data)) });
                        });
                    }

                    return 1;
                }
            }

            return null;
        }
    }
}