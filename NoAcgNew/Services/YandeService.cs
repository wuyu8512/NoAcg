using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NoAcgNew.Services
{
    public class YandeService
    {
        private static readonly Random Random = new();
        private readonly WebProxy _proxy;

        public YandeService(WebProxy proxy)
        {
            _proxy = proxy;
        }

        public YandeService()
        {
        }

        public async ValueTask<int> GetTagsPageAsync(string tag)
        {
            var client = new WebClient {Proxy = _proxy};
            var html = await client.DownloadStringTaskAsync("https://yande.re/post?tags=" + tag);
            ;
            var regex = new Regex("(\\d+)</a> <a [^<>]+?>Next");
            if (regex.IsMatch(html))
            {
                return int.Parse(regex.Match(html).Groups[1].Value);
            }

            var regex1 = new Regex("alt=\"Rating: (Explicit|Safe|Questionable).*?href=\"(.*?)\">");
            return regex1.IsMatch(html) ? 1 : 0;
        }

        public async ValueTask<(byte[] data, string imgRating)> GetImageByTagsAsync(string tag, int maxpage = 20,
            int rating = 7)
        {
            return await GetImgAsync($"https://yande.re/post?page={Random.Next(1, maxpage)}&tags={tag}", rating);
        }

        public async ValueTask<(byte[] data, string imgRating)> GetHotImgAsync(int rating = 7)
        {
            return await GetImgAsync("https://yande.re/post/popular_recent", rating);
        }

        private async ValueTask<(byte[] data, string imgRating)> GetImgAsync(string url, int rating = 7)
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

            var client = new WebClient {Proxy = _proxy};
            var regex = new Regex(text);
            var @string = await client.DownloadStringTaskAsync(url);
            var matchCollection = regex.Matches(@string);
            if (matchCollection.Count <= 0) throw new Exception("没有匹配到任何图片，请检查页数或网络设置");
            var i = Random.Next(0, matchCollection.Count - 1);
            var imgRating = matchCollection[i].Groups[1].Value;
            return (await client.DownloadDataTaskAsync(matchCollection[i].Groups[2].Value), imgRating);
        }
    }
}