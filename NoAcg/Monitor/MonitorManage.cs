using NoAcg.Models;
using Sora.Entities.Base;
using Sora.Entities.CQCodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Wuyu.Tool.Common;
using Wuyu.Tool.Web.HttpHelper;

namespace NoAcg.Monitor
{
	public class MonitorManage
	{
		private readonly TweeterMonitorConfig _config;
		private readonly SoraApi _soraApi;

		public MonitorManage(TweeterMonitorConfig config, SoraApi soraApi)
		{
			_config = config;
			_soraApi = soraApi;
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
						var data = HttpNet.Get(item["media_url_https"].ToString(), proxy: _config.Proxy);
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
									var data = HttpNet.Get(mp4["url"].ToString(), proxy: _config.Proxy);
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
									var data = HttpNet.Get(mp4["url"].ToString(), proxy: _config.Proxy);
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

		public void OpenTwitterMonitor()
		{
			var function = new Action<TweeterMonitor, Tweet>(async delegate (TweeterMonitor sender, Tweet tweet)
			{
				var content = GetTweetContent(tweet);
				content.Insert(0, CQCode.CQText($"您监控的{sender.Mark}有新的推文了\n"));
				var sendConfig = _config.Items.Single(c => c.Mark.ToString() == sender.Mark);
				if (sendConfig.Group != null)
				{
					foreach (var item in sendConfig.Group)
					{
						await _soraApi.SendGroupMessage(item, content);
					}
				}

				if (sendConfig.Private != null)
				{
					foreach (var item in sendConfig.Private)
					{
						await _soraApi.SendPrivateMessage(item, content);
					}
				}
			});
			var client = new WebClient();
			Twitter twitter = new Twitter(ref client);
			foreach (var tw in _config.Items.Select(item => new TweeterMonitor(item.Mark.ToString(), twitter) { TimeInterval = 60 }))
			{
				tw.NewTweetEvent += function;
				tw.Start();
			}
		}
	}
}