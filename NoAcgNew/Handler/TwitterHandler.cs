﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NoAcgNew.Core;
using NoAcgNew.Services;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;
using Wuyu.Tool.Common;
using Wuyu.Tool.Web.HttpHelper;

namespace NoAcgNew.Handler
{
    public class TwitterHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<TwitterHandler> _logger;
        private readonly GlobalService _globalService;
        private readonly IServiceProvider _provider;
        
        public TwitterHandler(EventManager eventManager,
            ILogger<TwitterHandler> logger, GlobalService globalService, IServiceProvider provider)
        {
            _eventManager = eventManager;
            _logger = logger;
            eventManager.OnGroupMessage += OnGroupMessage;
            _globalService = globalService;
            _provider = provider;
        }
        
        private async ValueTask<GroupMsgQuickOperation> OnGroupMessage(GroupMsgEventArgs args, IOneBotApi api)
        {
            return 0;
        }
        
	    private List<CQCode> GetTweetContent(Tweet tweet)
		{
			var temp = new List<CQCode> { CQCode.CQText(tweet.Content) };
			var img = new List<CQCode>();
			if (tweet.Media != null)
			{
				foreach (var item in tweet.Media)
				{
					try
					{
						var data = HttpNet.Get(item["media_url_https"].ToString(), proxy: _globalService.WebProxy);
						img.Add(CQCode.CQImage("base64://" + Convert.ToBase64String(data), useCache: true));
					}
					catch (Exception e)
					{
						img.Add(CQCode.CQText($"Error: {e.Message}"));
					}

					switch (item["type"].ToString())
					{
						case "photo":
							{
								break;
							}
						case "video":
							{
								var mp4 = item["video_info"]["variants"]
									.FirstOrDefault(video => video["content_type"].ToString() == "video/mp4");

								if (mp4 != null)
								{
									img.Add(CQCode.CQText(mp4["url"].ToString()));
									var data = HttpNet.Get(mp4["url"].ToString(), proxy: _globalService.WebProxy);
									var tempPath = AppDomain.CurrentDomain.BaseDirectory + "cache\\" + HashHelp.MD5Encrypt(data);
									File.WriteAllBytes(tempPath, data);
									img.Add(CQCode.CQVideo(tempPath, useCache: true));
								}
								else img.Add(CQCode.CQText(item["video_info"]["variants"][0]["url"].ToString()));
								break;
							}
						case "animated_gif":
							{
								var mp4 = item["video_info"]["variants"]
									.FirstOrDefault(video => video["content_type"].ToString() == "video/mp4");
								if (mp4 != null)
								{
									img.Add(CQCode.CQText(mp4["url"].ToString()));
									var data = HttpNet.Get(mp4["url"].ToString(), proxy: _globalService.WebProxy);
									var tempPath = AppDomain.CurrentDomain.BaseDirectory + "cache\\" + HashHelp.MD5Encrypt(data);
									File.WriteAllBytes(tempPath, data);
									img.Add(CQCode.CQVideo(tempPath, useCache: true));
								}
								else img.Add(CQCode.CQText(item["video_info"]["variants"][0]["url"].ToString()));
								break;
							}
					}
				}
			}

			if (tweet.IsOnlyRetweet)
			{
				if (tweet.Retweet == null)
				{
					return new List<CQCode> { CQCode.CQText("error") };
				}
				else
				{
					var a = new List<CQCode> { CQCode.CQText(tweet.Retweet.UserName + "：\n") };
					a.AddRange(GetTweetContent(tweet.Retweet));
					return a;
				}
			}
			else
			{
				var time = CQCode.CQText("\n发送时间：" + tweet.CreatTime.ToString("yyyy-MM-dd HH:mm"));
				if (tweet.Retweet == null)
				{
					temp.AddRange(img);
					temp.Add(time);
					return temp;
				}
				else
				{
					temp.AddRange(img);
					temp.Add(time);
					temp.Add(CQCode.CQText("\n" + tweet.Retweet.UserName + "：\n"));
					temp.AddRange(GetTweetContent(tweet.Retweet));
					return temp;
				}
			}
		}
    }
}