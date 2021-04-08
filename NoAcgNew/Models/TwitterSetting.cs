using System;
using System.Collections.Generic;

namespace NoAcgNew.Models
{
    public class TwitterSetting
    {
        public Dictionary<string, MonitorSetting> Monitor;
        
        public class MonitorSetting
        {
            public string Name { get; set; }
            public long[] Group { get; set; }
            public long[] Private { get; set; }
            public bool Enable { get; set; }
        }
    }
}