using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NoAcgNew.Core;
using NoAcgNew.Services;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Expansion;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Handler
{
    public class BiliBiliHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<BiliBiliHandler> _logger;
        private readonly GlobalService _globalService;
        private readonly IServiceProvider _provider;

        public BiliBiliHandler(EventManager eventManager,
            ILogger<BiliBiliHandler> logger, GlobalService globalService, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _logger = logger;
            eventManager.OnGroupMessage += HandlerExpansion.ToGroupHandler(Handler);
            eventManager.OnPrivateMessage += HandlerExpansion.ToPrivateHandler(Handler);
            _globalService = globalService;
            _provider = provider;
        }
        
        private async ValueTask<(int, BaseMsgQuickOperation)?> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            if (args.RawMessage.Equals(_globalService.BiliSetting.HotCos.Command,
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (BiliApi.GetCosHot(out var urls))
                {
                    var cqCodes = urls.Select(url => CQCode.CQImage(url)).ToArray();
                    return (1, cqCodes);
                }
            }

            return (0, null);
        }
    }
}