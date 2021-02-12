using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Wuyu.Tool.Web.HttpHelper;

namespace NoAcg.Models
{
    public class Yande
    {
        private static readonly Random Random = new Random();
        public WebProxy Proxy { get; set; } = null;
        public int Timeout { get; set; } = 10000;

        public Yande(YandeConfig config)
        {
            Proxy = config.Proxy;
            Timeout = config.Timeout;
        }

        public Yande()
        {
        }

        public int GetTagsPage(string tag)
        {
            var html = Encoding.UTF8.GetString(HttpNet.Get("https://yande.re/post?tags=" + tag, null, null, null,
                Timeout, Proxy));
            var regex = new Regex("(\\d+)</a> <a [^<>]+?>Next");
            if (regex.IsMatch(html))
            {
                return int.Parse(regex.Match(html).Groups[1].Value);
            }

            var regex1 = new Regex("alt=\"Rating: (Explicit|Safe|Questionable).*?href=\"(.*?)\">");
            return regex1.IsMatch(html) ? 1 : 0;
        }

        public byte[] GetImageByTags(string tag, out string imgRating, int maxpage = 20, int rating = 7)
        {
            return GetImg($"https://yande.re/post?page={Random.Next(1, maxpage)}&tags={tag}", out imgRating, rating);
        }

        public byte[] GetHotImg(out string imgRating, int rating = 7)
        {
            return GetImg("https://yande.re/post/popular_recent", out imgRating, rating);
        }

        private byte[] GetImg(string url, out string imgRating, int rating = 7)
        {
            var text = "alt=\"Rating: (Explicit|Safe|Questionable).*?href=\"(.*?)\">";
            if ((rating & 1) != 1)
            {
                text = text.Replace("|Safe", string.Empty);
            }

            if ((rating & 2) != 2)
            {
                text = text.Replace("|Questionable", string.Empty);
            }

            if ((rating & 4) != 4)
            {
                text = text.Replace("Explicit|", string.Empty);
            }

            var regex = new Regex(text);
            var @string = Encoding.UTF8.GetString(HttpNet.Get(url, null, null, null, Timeout, Proxy));
            var matchCollection = regex.Matches(@string);
            if (matchCollection.Count <= 0) throw new Exception("没有匹配到任何图片，请检查页数或网络设置");
            var i = Random.Next(0, matchCollection.Count - 1);
            imgRating = matchCollection[i].Groups[1].Value;
            return HttpNet.Get(matchCollection[i].Groups[2].Value, null, null, null, Timeout, Proxy);
        }
    }
}