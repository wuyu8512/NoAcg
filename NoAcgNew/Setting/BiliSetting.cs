namespace NoAcgNew.Setting
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