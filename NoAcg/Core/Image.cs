using Microsoft.Extensions.DependencyInjection;
using NoAcg.Model;
using Sora.Entities.CQCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool.Web.HttpHelper;

namespace NoAcg.Core
{
    internal class Image
    {
        private static readonly string[] Urls = new[]
        {
            "http://api.mtyqx.cn/api/random.php", "http://www.dmoe.cc/random.php",
            "https://www.320nle.com/acggirl/acgurl.php"
        };

        private static Random random = new Random();
        private readonly Yande _yande;
        private readonly dynamic _eventArgs;

        public Image(IServiceProvider service, dynamic eventArgs)
        {
            _yande = service.GetService<Yande>();
            _eventArgs = eventArgs;
        }

        private static byte[] ChangeMd5(IEnumerable<byte> data)
        {
            return data.Concat(BitConverter.GetBytes(random.Next(0, 255))).ToArray();
        }

        public CQCode GetHotImg(int rating = 7, bool 防止和谐 = true, bool 发送提示 = true)
        {
            if (发送提示) _eventArgs.Reply(CQCode.CQText("少女祈祷中..."));
            try
            {
                var data = _yande.GetHotImg(out string imgRating, rating);
                if (防止和谐 && (imgRating == "Explicit" || imgRating == "Questionable")) data = ChangeMd5(data);
                return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
            }
            catch (Exception e)
            {
                return CQCode.CQText("请求发生了错误：\n" + e.Message);
            }
        }

        public async void GetImgByTag(string tag, int count = 1, bool 防止和谐 = true,
            bool 发送提示 = true)
        {
            if (发送提示) _eventArgs.Reply(CQCode.CQText("少女祈祷中..."));
            int page = _yande.GetTagsPage(tag);
            if (page == 0) _eventArgs.Reply(CQCode.CQText("发生了未知错误"));
            else
            {
                var tasks = new List<Task>();
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            var data = _yande.GetImageByTags(tag, out string imgRating, page > 20 ? 20 : page);
                            if (防止和谐 && (imgRating == "Explicit" || imgRating == "Questionable"))
                                data = ChangeMd5(data);
                            _eventArgs.Reply(CQCode.CQImage("base64://" + Convert.ToBase64String(data)));
                        }
                        catch (Exception e)
                        {
                            _eventArgs.Reply(CQCode.CQText("请求发生了错误：\n" + e.Message));
                        }
                    }));
                }

                await Task.WhenAll(tasks.ToArray());
            }
        }

        public CQCode GetRandom()
        {
            return CQCode.CQImage(Urls[random.Next(Urls.Length)], useCache: false);
        }

        public CQCode GetRandCos()
        {
            //https://moe.ci/api.php https://moe.ci/Store/03-808P/0448.jpg
            var url = HttpNet.GetRedirectUrl("https://moe.ci/api.php");
            return CQCode.CQImage(url, useCache: true);
        }

        public object GetCosHot()
        {
            if (BiliBili.GetCosHot(out string[] urls))
            {
                return urls.Select(url => CQCode.CQImage(url));
            }
            else return CQCode.CQText("获取失败");
        }
    }
}