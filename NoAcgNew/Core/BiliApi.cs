using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wuyu.Tool.Common;
using Wuyu.Tool.Web.HttpHelper;

namespace NoAcgNew.Core
{
    public class BiliApi
    {
        private static readonly Random Random = new();

        public static async ValueTask<string[]> GetCosHotAsync()
        {
            var client = new HttpClient();

            var result = await client.GetStringAsync(
                $"https://api.vc.bilibili.com/link_draw/v2/Photo/list?category=cos&type=hot&page_num=1&page_size=20");
            var totalCount = JObject.Parse(result)["data"]["total_count"].ToObject<int>();
            totalCount = (int) Math.Ceiling(totalCount / 20.0) - 1;

            var @string =
                await client.GetStringAsync(
                    $"https://api.vc.bilibili.com/link_draw/v2/Photo/list?category=cos&type=hot&page_num={Random.Next(0, totalCount)}&page_size=20");
            if (string.IsNullOrWhiteSpace(@string)) return Array.Empty<string>();

            var jObject = JObject.Parse(@string);
            if (jObject["code"].ToObject<int>() != 0) return Array.Empty<string>();

            var jToken = jObject["data"]["items"];
            if (!jToken.Any()) return Array.Empty<string>();

            var num = Random.Next(0, jToken.Count() - 1);
            var jToken2 = jToken[num]["item"]["pictures"];
            return jToken2.Select(j => j["img_src"].ToString()).ToArray();
        }
    }
}