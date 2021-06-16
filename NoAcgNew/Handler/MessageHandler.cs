using Microsoft.Extensions.Logging;
using Wuyu.OneBot;


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
                return null;
            };
            _eventManager.OnGroupMessage += async (args, api) =>
            {
                _logger.LogDebug(args.MessageId, args.RawMessage);
                return null;
            };
        }
    }
}