using Newtonsoft.Json;
using Wuyu.OneBot.Converter;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 回复
    /// </summary>
    public struct Reply
    {
        #region 属性

        /// <summary>
        /// At目标UID
        /// 为<see langword="null"/>时为At全体
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "id")]
        public int Traget { get; internal set; }

        #endregion
    }
}