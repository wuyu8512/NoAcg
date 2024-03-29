﻿using System;
using System.Threading.Tasks;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;

namespace Wuyu.OneBot.Expansion
{
    public static class HandlerExpansion
    {
        public static EventManager.EventCallBackHandler<GroupMsgEventArgs, GroupMsgQuickOperation>
            ToGroupHandler(
                this Func<BaseMessageEventArgs, IOneBotApi, ValueTask<EventResult<BaseMsgQuickOperation>>> handler)
        {
            return async (args, api) =>
            {
                var result = await handler(args, api);
                if (result == null) return null;
                if (result.Operation == null) return new(result.Code);
                else return new(result.Code) { Operation = new(result.Operation) };
            };
        }

        public static EventManager.EventCallBackHandler<PrivateMsgEventArgs, PrivateMsgQuickOperation>
            ToPrivateHandler(
                this Func<BaseMessageEventArgs, IOneBotApi, ValueTask<EventResult<BaseMsgQuickOperation>>> handler)
        {
            return async (args, api) =>
            {
                var result = await handler(args, api);
                if (result == null) return null;
                if (result.Operation == null) return new(result.Code);
                else return new(result.Code) { Operation = new(result.Operation) };
            };
        }
    }
}