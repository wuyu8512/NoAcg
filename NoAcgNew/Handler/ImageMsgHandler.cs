using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoAcgNew.Services;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Handler
{
    public class ImageMsgHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<ImageMsgHandler> _logger;
        private readonly GlobalService _globalService;
        private readonly IServiceProvider _provider;

        public ImageMsgHandler(EventManager eventManager,
            ILogger<ImageMsgHandler> logger, GlobalService globalService, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _logger = logger;
            eventManager.OnGroupMessage += OnGroupMessage;
            eventManager.OnPrivateMessage += OnPrivateMessage;
            _globalService = globalService;
            _provider = provider;
        }

        private async ValueTask<GroupMsgQuickOperation> OnGroupMessage(GroupMsgEventArgs args, IOneBotApi api)
        {
            var result = await Handler(args, api);
            return result is null ? null : new GroupMsgQuickOperation(result);
        }

        private async ValueTask<PrivateMsgQuickOperation> OnPrivateMessage(PrivateMsgEventArgs args, IOneBotApi api)
        {
            var result = await Handler(args, api);
            return result is null ? null : new PrivateMsgQuickOperation(result);
        }

        private async ValueTask<BaseMsgQuickOperation> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            long? groupId = null;
            long? userId = null;
            if (args is GroupMsgEventArgs groupArgs) groupId = groupArgs.GroupId;
            else userId = args.UserId;

            var yandeService = ActivatorUtilities.CreateInstance<YandeService>(_provider, _globalService.WebProxy);
            if (args.RawMessage == _globalService.YandeSetting.HotImg.Command)
            {
                var (data, rating) = await yandeService.GetHotImgAsync(_globalService.YandeSetting.HotImg.Rating);
                var replay = CQCode.CQImage("base64://" + Convert.ToBase64String(data));
                return (replay, 1);
            }

            foreach (var customTags in _globalService.YandeSetting.CustomTags)
            {
                if (args.RawMessage == customTags.Command)
                {
                    var page = await yandeService.GetTagsPageAsync(customTags.Tag);
                    for (var i = 0; i < customTags.Count; i++)
                    {
                        var _ = Task.Run(async () =>
                        {
                            var (data, rating) =
                                await yandeService.GetImageByTagsAsync(customTags.Tag, page, customTags.Rating);
                            await api.SendMsg(userId, groupId,
                                new[] {CQCode.CQImage("base64://" + Convert.ToBase64String(data))});
                        });
                    }

                    return 1;
                }
            }

            return null;
        }
    }
}