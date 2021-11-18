using NoAcgNew.Enumeration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public static async ValueTask<CQCode> Image(string uri, CQFileType type = default, IHttpClientFactory httpClientFactory = default)
        {
            HttpClient client;
            byte[] data;
            switch (type)
            {
                case CQFileType.Url:
                    return CQCode.CQImage(uri);
                case CQFileType.Base64:
                    client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    data = await client.GetByteArrayAsync(uri);
                    return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
                case CQFileType.File:
                    client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    data = await client.GetByteArrayAsync(uri);
                    var filePath = ImageCachePath + HashHelp.MD5Encrypt(data);
                    await File.WriteAllBytesAsync(filePath, data);
                    return CQCode.CQImage(new Uri(filePath).AbsoluteUri);
                default:
                    return null;
            }
        }
        
        public static async ValueTask<CQCode> Video(string uri, CQFileType type = default, IHttpClientFactory httpClientFactory = default)
        {
            switch (type)
            {
                case CQFileType.Url:
                    return CQCode.CQVideo(uri);
                case CQFileType.Base64:
                    throw new NotSupportedException("Video不支持Base64发送");
                case CQFileType.File:
                    var client = httpClientFactory == default ? new HttpClient() : httpClientFactory.CreateClient("default");
                    var data = await client.GetByteArrayAsync(uri);
                    var filePath = VideoCachePath + HashHelp.MD5Encrypt(data);
                    await File.WriteAllBytesAsync(filePath, data);
                    return CQCode.CQImage(new Uri(filePath).AbsoluteUri);
                default:
                    return null;
            }
        }
    }
}