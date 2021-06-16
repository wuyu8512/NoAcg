using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NoAcgNew.Enumeration
{
    [DefaultValue(Url)]
    public enum CQFileType
    {
        Url,
        File,
        Base64
    }
}
