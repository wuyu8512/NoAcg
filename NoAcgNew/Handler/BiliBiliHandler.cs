using Microsoft.Extensions.Logging;
using NoAcgNew.Attributes;
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
using Wuyu.OneBot.Models.QuickOperation;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Handler
{
    [Handler]
    public class BiliBiliHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<BiliBiliHandler> _logger;
        private readonly ConfigService _globalService;
        private readonly IServiceProvider _provider;

        public BiliBiliHandler(EventManager eventManager,
            ILogger<BiliBiliHandler> logger, ConfigService globalService, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _logger = logger;
            eventManager.OnGroupMessage += HandlerExpansion.ToGroupHandler(Handler);
            eventManager.OnPrivateMessage += HandlerExpansion.ToPrivateHandler(Handler);
            _globalService = globalService;
            _provider = provider;
        }

        private async ValueTask<EventResult<BaseMsgQuickOperation>> Handler(BaseMessageEventArgs args, IOneBotApi api)
        {
            if (args.RawMessage.Equals(_globalService.BiliSetting.HotCos.Command,
                StringComparison.CurrentCultureIgnoreCase))
            {
                var result = await BiliApi.GetCosHotAsync();
                if (result.Any())
                {
                    _logger.LogDebug("{Command}数量：{Count}", _globalService.BiliSetting.HotCos.Command, result.Length);
                    var cqCodes = new List<CQCode>();
                    var tasks = result.Select(url => Task.Run(async () =>
                    {
                        try
                        {
                            cqCodes.Add(await CQHelper.Image(url, CQFileType.Base64));
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning("[SendImage]下载图片失败：{Url}\r\nError: {Error}", e.ToString(), url);
                        }
                    }));

                    await Task.WhenAll(tasks);
                    return (1, cqCodes.ToArray());
                }
            }

            return null;
        }
    }
}