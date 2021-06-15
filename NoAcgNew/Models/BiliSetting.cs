namespace NoAcgNew.Models
{
    public class BiliSetting
    {
        public HotCosSetting HotCos { get; set; }

        public class HotCosSetting
        {
            public string Command { get; init; }
        }
    }
}