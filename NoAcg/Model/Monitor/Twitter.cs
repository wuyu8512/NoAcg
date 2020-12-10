using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tool;

namespace NoAcg.Model.Monitor
{
    public class Twitter
    {
        private WebClient _webClient;
        public string Authorization => _webClient.Headers["authorization"];
        public string Path { get; private set; }
        public string Token => _webClient.Headers["x-guest-token"];
        public static TweetConfig TweetCache { get; set; } = new TweetConfig();

        public Twitter(ref WebClient webClient, bool userCache = true)
        {
            this._webClient = webClient;
            lock (TweetCache)
            {
                if (userCache)
                {
                    if (string.IsNullOrWhiteSpace(TweetCache.Authorization) || string.IsNullOrWhiteSpace(TweetCache.Path))
                    {
                        GetAuthorization();
                    }
                    else
                    {
                        webClient.Headers["authorization"] = TweetCache.Authorization;
                        Path = TweetCache.Path;
                    }

                    if (string.IsNullOrWhiteSpace(TweetCache.Token)) GetToken();
                    else webClient.Headers["x-guest-token"] = TweetCache.Token;
                }
                else
                {
                    GetAuthorization();
                    GetToken();
                }

                TweetCache.Authorization = this.Authorization;
                TweetCache.Path = this.Path;
                TweetCache.Token = this.Token;
            }
        }

        protected void GetAuthorization()
        {
            string @string;
            try
            {
                lock (_webClient)
                {
                    @string = Encoding.UTF8.GetString(_webClient.DownloadData("https://twitter.com/"));
                }
            }
            catch (WebException e)
            {
                LogHelp.WriteLine(e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    LogHelp.WriteLine("进入等待");
                    Task.Delay(5 * 60 * 1000).Wait();
                }

                return;
            }

            Regex regex = new Regex("https(.*?)/main\\.(.*?)\\.js");
            string text = regex.Match(@string).Value;
            string string2;
            lock (_webClient)
            {
                string2 = Encoding.UTF8.GetString(_webClient.DownloadData(text));
            }
            Regex regex2 = new Regex("\"(A{5,}.*?)\"");
            string value = "Bearer " + regex2.Match(string2).Groups[1].Value;
            _webClient.Headers["authorization"] = value;
            Path = new Regex("queryId:\"([a-zA-Z0-9-_]*?)\",operationName:\"UserByScreenName\"").Match(string2)
                .Groups[1].Value;
        }

        protected void GetToken()
        {
            string value2 = string.Empty;
            //webClient.Headers["x-guest-token"] = value2;
            try
            {
                lock (_webClient)
                {
                    value2 = JObject.Parse(Encoding.UTF8.GetString(
                        _webClient.UploadData("https://api.twitter.com/1.1/guest/activate.json", new byte[0])))[
                        "guest_token"].ToString();
                }
                _webClient.Headers["x-guest-token"] = value2;
            }
            catch (WebException)
            {
                _webClient.Dispose();
                _webClient = new WebClient() {Proxy = _webClient.Proxy};
                GetAuthorization();
            }
        }

        public string GetUserID(string userName)
        {
            string text2 =
                $"https://api.twitter.com/graphql/{Path}/UserByScreenName?variables=%7B%22screen_name%22%3A%22{userName}%22%2C%22withHighlightedLabel%22%3Afalse%7D";
            lock (_webClient)
            {
                return JObject.Parse(Encoding.UTF8.GetString(_webClient.DownloadData(text2)))["data"]["user"]["rest_id"]
                    .ToString();
            }
        }

        public Tweet[] GetTweets(string userId)
        {
            string address =
                $"https://api.twitter.com/2/timeline/profile/{userId}.json?tweet_mode=extended&simple_quoted_tweet=true";
            string string3;
            try
            {
                lock (_webClient)
                {
                    string3 = Encoding.UTF8.GetString(_webClient.DownloadData(address));
                }
            }
            catch (WebException e)
            {
                LogHelp.WriteLine(e.ToString());
                if (int.TryParse(e.Response.Headers["status"], out int status))
                {
                    LogHelp.WriteLine(e.ToString());
                    LogHelp.WriteLine("状态码", status.ToString());
                }

                GetToken();
                return null;
            }

            var json = JObject.Parse(string3);
            List<Tweet> tweets = new List<Tweet>();
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
            var temp = json["globalObjects"]["tweets"][tweetId] as JObject;
            if (temp != null)
            {
                tweet = new Tweet();
                tweet.ID = tweetId;
                tweet.CreatTime = DateTime.ParseExact(temp["created_at"].ToString(), "ddd MMM dd HH:mm:ss zzz yyyy",
                    CultureInfo.InvariantCulture);
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
                    var full_text = temp["full_text"].ToString();
                    if (!full_text.StartsWith("RT @"))
                    {
                        tweet.Content = full_text;
                        tweet.IsOnlyRetweet = false;
                    }
                    else tweet.IsOnlyRetweet = true;

                    ParseTweet(json, retweetId, out Tweet retweet);
                    tweet.Retweet = retweet;
                }
                else
                {
                    var data = Encoding.UTF32.GetBytes(temp["full_text"].ToString());
                    var end = temp["display_text_range"][1].ToObject<int>() * 4;
                    tweet.Content = Encoding.UTF32.GetString(data[0..end]);
                    if (temp.ContainsKey("quoted_status_id_str"))
                    {
                        ParseTweet(json, temp["quoted_status_id_str"].ToString(), out Tweet retweet);
                        tweet.Retweet = retweet;
                    }
                }
            }
            else tweet = null;
        }
    }

    public class Tweet
    {
        public string ID { get; set; }
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