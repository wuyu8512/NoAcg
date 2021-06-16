using Microsoft.Extensions.Logging;
using NoAcgNew.Core;
using NoAcgNew.Enumeration;
using NoAcgNew.Helper;
using NoAcgNew.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private async ValueTask<BaseMsgQuickOperation> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            if (args.RawMessage.Equals(_globalService.BiliSetting.HotCos.Command,
                StringComparison.CurrentCultureIgnoreCase))
            {
                var result = await BiliApi.GetCosHotAsync();
                if (result.Any())
                {
                    _logger.LogDebug("数量：{Count}", result.Length);
                    var cqCodes = new List<CQCode>();
                    var tasks = result.Select(url => Task.Run(async () =>
                    {
                        cqCodes.Add(await CQHelper.Image(url, CQFileType.Base64));
                    }));

                    await Task.WhenAll(tasks);
                    return (1, cqCodes.ToArray());
                }
            }

            return null;
        }
    }
}