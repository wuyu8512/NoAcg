using NoAcgNew.Enumeration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Wuyu.OneBot.Entities.CQCodes;
using Wuyu.Tool.Common;

namespace NoAcgNew.Helper
{
    public static class CQHelper
    {
        private static readonly string ImageCachePath;
        private static readonly string VideoCachePath;

        static CQHelper()
        {
            ImageCachePath = AppDomain.CurrentDomain.BaseDirectory + "Cache/Image/";
            VideoCachePath = AppDomain.CurrentDomain.BaseDirectory + "Cache/Video/";
            Directory.CreateDirectory(ImageCachePath);
            Directory.CreateDirectory(VideoCachePath);
        }

        public static async ValueTask<CQCode> Image(string url, CQFileType type = default, IHttpClientFactory httpClientFactory = default)
        {
            HttpClient client;
            byte[] data;
            switch (type)
            {
                case CQFileType.Url:
                    return CQCode.CQImage(url);
                case CQFileType.Base64:
                    client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    data = await client.GetByteArrayAsync(url);
                    return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
                case CQFileType.File:
                    Uri uri = new(url);
                    client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    data = await client.GetByteArrayAsync(uri);
                    var filePath = ImageCachePath + HashHelp.MD5Encrypt(data) + Path.GetExtension(uri.Segments.Last());
                    await File.WriteAllBytesAsync(filePath, data);
                    return CQCode.CQImage(new Uri(filePath).AbsoluteUri);
                default:
                    return null;
            }
        }

        public static async ValueTask<CQCode> Video(string url, CQFileType type = default, string cover = default, IHttpClientFactory httpClientFactory = default)
        {
            switch (type)
            {
                case CQFileType.Url:
                    return CQCode.CQVideo(url, cover);
                case CQFileType.Base64:
                    throw new NotSupportedException("Video不支持Base64发送");
                case CQFileType.File:
                    Uri uri = new(url);
                    var client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    var data = await client.GetByteArrayAsync(uri);
                    var filePath = VideoCachePath + HashHelp.MD5Encrypt(data) + Path.GetExtension(uri.Segments.Last());
                    await File.WriteAllBytesAsync(filePath, data);
                    return CQCode.CQVideo(new Uri(filePath).AbsoluteUri, cover);
                default:
                    return null;
            }
        }
    }
}