using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace NoAcgNew.Handler
{
    public class MessageHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(EventManager eventManager,
            ILogger<MessageHandler> logger)
        {
            _eventManager = eventManager;
            _logger = logger;
            
            _eventManager.OnPrivateMessage += async (args, api) =>
            {
                _logger.LogDebug(args.MessageId, args.RawMessage);
                return (0, CQCode.CQText(args.RawMessage));
            };
            _eventManager.OnGroupMessage += async (args, api) =>
            {
                _logger.LogDebug(args.MessageId, args.RawMessage);
                return null;
            };
        }
    }
}