using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public static bool GetCosHot(out string[] urls)
        {
            urls = Array.Empty<string>();
            var @string = Encoding.UTF8.GetString(HttpNet.Get(
                $"https://api.vc.bilibili.com/link_draw/v2/Photo/list?category=cos&type=hot&page_num={Random.Next(0, 15)}&page_size=20"));
            if (string.IsNullOrWhiteSpace(@string))
            {
                return false;
            }

            JObject jObject;
            try
            {
                jObject = JObject.Parse(@string);
            }
            catch (Exception)
            {
                return false;
            }

            if (jObject["code"].ToObject<int>() != 0) return false;
            var jToken = jObject["data"]["items"];
            if (!jToken.Any())
            {
                return false;
            }

            var num = Random.Next(0, jToken.Count() - 1);
            var jToken2 = jToken[num]["item"]["pictures"];
            urls = jToken2.Select(j => j["img_src"].ToString()).ToArray();
            return true;
        }
    }
}