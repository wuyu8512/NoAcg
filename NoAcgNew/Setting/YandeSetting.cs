namespace NoAcgNew.Setting
{
    public class YandeSetting
    {
        public HotImgSetting HotImg { get; set; }
        public CustomTagsSetting[] CustomTags { get; set; }

        public class HotImgSetting
        {
            public string Command { get; init; }
            public int Rating { get; init; }
        }

        public class CustomTagsSetting : HotImgSetting
        {
            public string Tag { get; init; }
            public int Count { get; init; }
        }
    }
}