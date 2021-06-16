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
        private static string ImageCachePath;

        static CQHelper()
        {
            ImageCachePath = AppDomain.CurrentDomain.BaseDirectory + "Cache/Image/";
            Directory.CreateDirectory(ImageCachePath);
        }

        public static async ValueTask<CQCode> Image(string uri, CQImageType type = default, HttpClientHandler handler = default)
        {
            HttpClient client;
            switch (type)
            {
                case CQImageType.Url:
                    return CQCode.CQImage(uri);
                case CQImageType.Base64:
                    client = new HttpClient(handler, false);
                    var data = await client.GetByteArrayAsync(uri);
                    return CQCode.CQImage("base64://" + Convert.ToBase64String(data));
                case CQImageType.File:
                    client = new HttpClient(handler, false);
                    var filePath = ImageCachePath + TimeHelp.GetCurrentTimeUnix(true);
                    using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        using var imageStream = await client.GetStreamAsync(uri);
                        await imageStream.CopyToAsync(fileStream);
                        return CQCode.CQImage("file://" + filePath);
                    }
            }
            return null;
        }
    }
}