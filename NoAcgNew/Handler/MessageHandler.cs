using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NoAcgNew.Onebot;
using NoAcgNew.Onebot.Event;
using NoAcgNew.Onebot.Models;

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
			
			_logger.LogInformation("我被初始化了");
			
			//_eventManager.OnHeartBeatEvent += async (args, api) => { _logger.LogInformation(args.Time.ToString()); };
			_eventManager.OnLifeCycleEvent += async (args, api) =>
			{
				_logger.LogInformation(0,args.SubType);
				return 0;
			};
			_eventManager.OnPrivateMessage += async (args, api) =>
			{
				var (status,id) = await api.SendPrivateMsg(args.UserId, 0, args.MessageList);
				_logger.LogInformation(id.ToString());
				return null;
			};
			_eventManager.OnGroupMessage += async (args, api) =>
			{
				_logger.LogInformation(args.MessageId,args.RawMessage);
				return new GroupMsgReturn() {Reply = args.MessageList};
			};
		}
	}
}
