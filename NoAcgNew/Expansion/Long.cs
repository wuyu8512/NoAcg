using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoAcgNew.Expansion
{
	public static class Long
	{
		public static DateTime ToDateTime(this long timeStamp)
		{
			DateTime dtStart = (new DateTime(1970, 1, 1)).ToLocalTime();
			long lTime = (timeStamp * 10000000);
			TimeSpan toNow = new TimeSpan(lTime);
			DateTime targetDt = dtStart.Add(toNow);
			return targetDt;
		}
	}
}
