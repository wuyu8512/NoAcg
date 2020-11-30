using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tool.Web.HttpHelper;

namespace NoAcg.Model
{
    public static class BiliBili
    {
        private static readonly Random random = new Random();

        public static bool GetCosHot(out string[] urls)
        {
            urls = Array.Empty<string>();
            string @string = Encoding.UTF8.GetString(HttpNet.Get(
                $"https://api.vc.bilibili.com/link_draw/v2/Photo/list?category=cos&type=hot&page_num={random.Next(0, 15)}&page_size=20"));
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

            if (jObject["code"].ToObject<int>() == 0)
            {
                JToken jToken = jObject["data"]["items"];
                if (jToken.Count() == 0)
                {
                    return false;
                }

                int num = random.Next(0, jToken.Count() - 1);
                JToken jToken2 = jToken[num]["item"]["pictures"];
                List<string> str = new List<string>();

                for (int i = 0; i < jToken2.Count(); i++)
                {
                    str.Add(jToken2[i]["img_src"].ToString());
                }

                urls = str.ToArray();
                return true;
            }

            return false;
        }

        public static string GetName(long uid)
        {
            try
            {
                return JObject.Parse(Encoding.UTF8.GetString(
                        HttpNet.Get("https://api.bilibili.com/x/web-interface/card?mid=" + uid.ToString())))["data"][
                        "card"]
                    ["name"].ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}