using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoAcgNew.Expansion
{
	internal static class DateTimeExpansion
	{
		public static DateTime ToDateTime(this long timeStamp)
		{
			var dtStart = (new DateTime(1970, 1, 1)).ToLocalTime();
			var lTime = (timeStamp * 10000000);
			var toNow = new TimeSpan(lTime);
			var targetDt = dtStart.Add(toNow);
			return targetDt;
		}
	}
}
