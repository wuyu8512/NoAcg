using System;
using Newtonsoft.Json;

namespace NoAcgNew.Internal
{
    public static class MessageHelper
    {
        public static string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }
    }
}