using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wuyu.Tool;

namespace NoAcgNew.Core.Twitter
{
    public class TwitterApi
    {
        private readonly Lazy<HttpClient> _client;
        private static TweetConfig TweetCache { get; } = new();

        public TwitterApi(HttpMessageHandler handler)
        {
            _client = new Lazy<HttpClient>(() =>
            {
                var client = new HttpClient(handler, false);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
                InitAuthorizationAsync().Wait();
                return client;
            });
        }

        public async Task InitAuthorizationAsync()
        {
            if (string.IsNullOrWhiteSpace(TweetCache.Authorization) ||
                string.IsNullOrWhiteSpace(TweetCache.Path))
            {
                await GetAuthorizationAsync();
            }

            if (string.IsNullOrWhiteSpace(TweetCache.Token)) await GetTokenAsync();
        }

        private async ValueTask GetAuthorizationAsync()
        {
            string html;
            try
            {
                html = Encoding.UTF8.GetString(await _client.Value.GetByteArrayAsync("https://twitter.com/"));
            }
            catch (HttpRequestException e)
            {
                LogHelp.WriteLine(e.ToString());
                if (e.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    LogHelp.WriteLine("进入等待");
                    await Task.Delay(5 * 60 * 1000);
                }

                return;
            }

            var text = Regex.Match(html, "https([^\"]*?)/main\\.([^\"]*?)\\.js").Value;
            string string2;
            try
            {
                string2 = await _client.Value.GetStringAsync(text);
            }
            catch (HttpRequestException e)
            {
                LogHelp.WriteLine(e.ToString());
                return;
            }
            
            var value = "Bearer " + Regex.Match(string2, "\"(A{5,}.*?)\"").Groups[1].Value;
            TweetCache.Authorization = value;

            TweetCache.Path = new Regex("queryId:\"([a-zA-Z0-9-_]*?)\",operationName:\"UserByScreenName\"")
                .Match(string2)
                .Groups[1].Value;
            
            _client.Value.DefaultRequestHeaders.Remove("authorization");
            _client.Value.DefaultRequestHeaders.Add("authorization", TweetCache.Authorization);
        }

        private async ValueTask GetTokenAsync()
        {
            try
            {
                var response = await _client.Value.PostAsync("https://api.twitter.com/1.1/guest/activate.json",
                    new ByteArrayContent(Array.Empty<byte>()));
                var token = JObject.Parse(await response.Content.ReadAsStringAsync())["guest_token"]?.ToString();
                TweetCache.Token = token;
            }
            catch (HttpRequestException)
            {
                await GetAuthorizationAsync();
            }
            _client.Value.DefaultRequestHeaders.Remove("x-guest-token");
            _client.Value.DefaultRequestHeaders.Add("x-guest-token", TweetCache.Token);
        }

        public async ValueTask<string> GetUserIDAsync(string userName)
        {
            var url =
                $"https://api.twitter.com/graphql/{TweetCache.Path}/UserByScreenName?variables=%7B%22screen_name%22%3A%22{userName}%22%2C%22withHighlightedLabel%22%3Afalse%7D";
            var str = await _client.Value.GetStringAsync(url);
            var json = JObject.Parse(str);
            return json["data"]["user"]["result"]["rest_id"].ToString();
        }

        public async ValueTask<Tweet[]> GetTweetsAsync(string userId)
        {
            var address =
                $"https://api.twitter.com/2/timeline/profile/{userId}.json?tweet_mode=extended&simple_quoted_tweet=true";
            string string3;
            try
            {
                string3 = await _client.Value.GetStringAsync(address);
            }
            catch (HttpRequestException e)
            {
                await GetTokenAsync();
                return null;
            }

            var json = JObject.Parse(string3);
            var tweets = new List<Tweet>();
            foreach (var item in json["timeline"]["instructions"][0]["addEntries"]["entries"])
            {
                var entryId = item["entryId"].ToString();
                if (entryId.Contains("tweet"))
                {
                    var id = item["sortIndex"].ToString();
                    ParseTweet(json, id, out Tweet tweet);
                    tweets.Add(tweet);
                }
            }

            return tweets.ToArray();
        }

        private static void ParseTweet(JObject json, string tweetId, out Tweet tweet)
        {
            if (json["globalObjects"]["tweets"][tweetId] is JObject temp)
            {
                tweet = new Tweet
                {
                    Id = tweetId,
                    CreatTime = DateTime.ParseExact(temp["created_at"].ToString(), "ddd MMM dd HH:mm:ss zzz yyyy",
                        CultureInfo.InvariantCulture)
                };
                var isRetweet = temp.ContainsKey("retweeted_status_id_str");
                var userId = temp["user_id_str"].ToString();
                tweet.UserName = json["globalObjects"]["users"][userId]["name"].ToString();
                if (temp.ContainsKey("extended_entities"))
                {
                    var extended = temp["extended_entities"] as JObject;
                    if (extended.ContainsKey("media"))
                    {
                        tweet.Media = extended["media"] as JArray;
                    }
                }

                if (isRetweet)
                {
                    var retweetId = temp["retweeted_status_id_str"].ToString();
                    var fullText = temp["full_text"].ToString();
                    if (!fullText.StartsWith("RT @"))
                    {
                        tweet.Content = fullText;
                        tweet.IsOnlyRetweet = false;
                    }
                    else tweet.IsOnlyRetweet = true;

                    ParseTweet(json, retweetId, out var retweet);
                    tweet.Retweet = retweet;
                }
                else
                {
                    var data = Encoding.UTF32.GetBytes(temp["full_text"].ToString());
                    var end = temp["display_text_range"][1].ToObject<int>() * 4;
                    tweet.Content = Encoding.UTF32.GetString(data[0..end]);
                    if (temp.ContainsKey("quoted_status_id_str"))
                    {
                        ParseTweet(json, temp["quoted_status_id_str"].ToString(), out var retweet);
                        tweet.Retweet = retweet;
                    }
                }
            }
            else tweet = null;
        }
    }

    public class Tweet
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public JArray Media { get; set; }
        public DateTime CreatTime { get; set; }
        public string UserName { get; set; }
        public Tweet Retweet { get; set; }
        public bool IsOnlyRetweet { get; set; }
    }

    public class TweetConfig
    {
        public string Authorization;
        public string Path;
        public string Token;
    }
}