using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoAcgNew.Attributes;
using NoAcgNew.Core.Twitter;
using NoAcgNew.Enumeration;
using NoAcgNew.Helper;
using NoAcgNew.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Wuyu.OneBot;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.OneBot.Enumeration;
using Wuyu.OneBot.Interfaces;
using Wuyu.OneBot.Models.EventArgs.MessageEvent;
using Wuyu.OneBot.Models.QuickOperation;
using Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation;
using AngleSharp.Html.Parser;
using Newtonsoft.Json.Linq;

namespace NoAcgNew.Handler
{
    [Handler]
    public class TwitterHandler
    {
        private readonly EventManager _eventManager;
        private readonly ILogger<TwitterHandler> _logger;
        private readonly ConfigService _globalService;
        private readonly IServiceProvider _provider;
        private readonly IHttpClientFactory _httpClientFactory;
        private TweeterMonitorManage _manage;
        private Lazy<TwitterApi> _twitterApi;
        private IOneBotApi _api;
        private bool _isStart;

        public TwitterHandler(EventManager eventManager,
            ILogger<TwitterHandler> logger, ConfigService globalService, IServiceProvider provider, IHttpClientFactory httpClientFactory)
        {
            _eventManager = eventManager;
            _logger = logger;
            _globalService = globalService;
            _provider = provider;
            _httpClientFactory = httpClientFactory;

            eventManager.OnGroupMessage += OnGroupMessage;
            eventManager.OnConnection += OnConnection;
        }

        private async ValueTask OnConnection(IOneBotApi api)
        {
            _api = api;
            if (!_isStart)
            {
                foreach (var monitor in _globalService.TwitterSetting.Monitor.Where(monitor => monitor.Value.Enable))
                {
                    if (_manage == null)
                    {
                        //_twitterApi = new Lazy<TwitterApi>(() => new TwitterApi(_globalService.HttpClientProxyHandler));
                        _twitterApi = ActivatorUtilities.CreateInstance<Lazier<TwitterApi>>(_provider);
                        _manage = ActivatorUtilities.CreateInstance<TweeterMonitorManage>(_provider, _twitterApi);
                    }

                    _manage.StartNewMonitor(monitor.Key, CallBack);
                }

                _isStart = true;
            }
        }

        private async ValueTask<EventResult<GroupMsgQuickOperation>> OnGroupMessage(GroupMsgEventArgs args, IOneBotApi api)
        {
            return null;
        }
         
        private async ValueTask CallBack(TweeterMonitor monitor, Tweet tweet)
        {
            if (_api == null) return;
            var content = new List<CQCode>();
            await GetTweetContent(content, tweet);
            content.Insert(0, CQCode.CQText($"您订阅的{monitor.Name}有新推文了\n"));
            var video = content.Where(c => c.Type == CQCodeType.Video).ToArray();
            content = content.Where(c => c.Type != CQCodeType.Video).ToList();
            var sendConfig = _globalService.TwitterSetting.Monitor[monitor.Name];
            if (sendConfig.Group != null)
            {
                foreach (var item in sendConfig.Group)
                {
                    await Task.Delay(1000);
                    if (content.Any()) await _api.SendGroupMsg(item, content);
                    await Task.Delay(1000);
                    if (video.Any()) await _api.SendGroupMsg(item, video);
                }
            }

            if (sendConfig.Private != null)
            {
                foreach (var item in sendConfig.Private)
                {
                    await Task.Delay(1000);
                    if (content.Any()) await _api.SendPrivateMsg(item, null, content);
                    await Task.Delay(1000);
                    if (video.Any()) await _api.SendPrivateMsg(item, null, video);
                }
            }
        }

        private async ValueTask GetTweetContent(List<CQCode> codes, Tweet tweet)
        {
            //var temp = new List<CQCode> { CQCode.CQText(tweet.Content) };
            codes.Add(CQCode.CQText(tweet.Content));
            var img = new List<CQCode>();
            if (tweet.Media != null)
            {
                foreach (var item in tweet.Media)
                {
                    try
                    {
                        img.Add(await CQHelper.Image(item["media_url_https"].ToString(), CQFileType.Base64, _httpClientFactory));
                    }
                    catch (Exception e)
                    {
                        img.Add(CQCode.CQText($"Error: {e.Message}"));
                    }

                    switch (item["type"].ToString())
                    {
                        case "photo":
                            break;
                        case "video":
                        case "animated_gif":
                            var mp4 = item["video_info"]["variants"]
                                .FirstOrDefault(video => video["content_type"].ToString() == "video/mp4");
                            if (mp4 != null)
                            {
                                var videoUrl = mp4["url"].ToString();
                                img.Add(CQCode.CQText(videoUrl));
                                img.Add(await CQHelper.Video(videoUrl, CQFileType.File, null, _httpClientFactory));
                            }
                            else img.Add(CQCode.CQText(item["video_info"]["variants"][0]["url"].ToString()));

                            break;
                    }
                }
            }

            if (img.Count == 0 && tweet.Retweet == null)
            {
                if (tweet.Entities.TryGetValue("urls", out var urls))
                {
                    var temp = await TryGetUrlImage(((JArray)urls)[0]["expanded_url"].ToString(), tweet.UserId);
                    if (temp != null) codes.AddRange(temp);
                }
            }

            if (tweet.IsOnlyRetweet)
            {
                if (tweet.Retweet == null)
                {
                    //return new List<CQCode> { CQCode.CQText("error") };
                    codes.Add(CQCode.CQText("error"));
                }
                else
                {
                    //var a = new List<CQCode> { CQCode.CQText(tweet.Retweet.UserName + "：\n") };
                    //a.AddRange(await GetTweetContent(tweet.Retweet));
                    //return a;
                    codes.Add(CQCode.CQText(tweet.Retweet.UserName + "：\n"));
                    await GetTweetContent(codes, tweet.Retweet);
                }
            }
            else
            {
                var time = CQCode.CQText("\n发送时间：" + tweet.CreatTime.ToString("yyyy-MM-dd HH:mm"));
                if (tweet.Retweet == null)
                {
                    //temp.AddRange(img);
                    //temp.Add(time);
                    //return temp;
                    codes.AddRange(img);
                    codes.Add(time);
                }
                else
                {
                    //temp.AddRange(img);
                    //temp.Add(time);
                    //temp.Add(CQCode.CQText("\n" + tweet.Retweet.UserName + "：\n"));
                    //temp.AddRange(await GetTweetContent(tweet.Retweet));
                    //return temp;
                    codes.AddRange(img);
                    codes.Add(time);
                    codes.Add(CQCode.CQText("\n" + tweet.Retweet.UserName + "：\n"));
                    await GetTweetContent(codes, tweet.Retweet);
                }
            }
        }

        private async ValueTask<CQCode[]> TryGetUrlImage(string url,string id = null)
        {
            var client = _httpClientFactory.CreateClient("default");
            var html = await client.GetStringAsync(url);
            var parse = new HtmlParser();
            var doc = await parse.ParseDocumentAsync(html);

            if (id == "381813198")
            {
                var imgs = doc.QuerySelectorAll(".mce-content-body img").Select(x => x.GetAttribute("src")).ToList();
                var tasks = imgs.Select(x =>
                {
                    return Task.Run(async () =>
                    {
                        return await CQHelper.Image(x, CQFileType.Base64, _httpClientFactory);
                    });
                });
                await Task.WhenAll(tasks);
                return tasks.Select(x => x.Result).ToArray();
            }

            return null;
        }
    }
}