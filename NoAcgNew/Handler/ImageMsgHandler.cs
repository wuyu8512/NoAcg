using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoAcgNew.Services;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
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
            eventManager.OnGroupMessage += OnGroupMessage;
            eventManager.OnPrivateMessage += OnPrivateMessage;
            _globalService = globalService;
            _provider = provider;
        }

        private async ValueTask<GroupMsgQuickOperation> OnGroupMessage(GroupMsgEventArgs args, IOneBotApi api)
        {
            return new(await Handler(args, api));
        }

        private async ValueTask<PrivateMsgQuickOperation> OnPrivateMessage(PrivateMsgEventArgs args, IOneBotApi api)
        {
            return new(await Handler(args, api));
        }
        
        private async ValueTask<BaseMsgQuickOperation> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            var yandeService = ActivatorUtilities.CreateInstance<YandeService>(_provider, _globalService.WebProxy);
            if (args.RawMessage == "热门图片")
            {
                var (data, rating) = await yandeService.GetHotImgAsync();
                return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
            }

            return 0;
        }
    }
}