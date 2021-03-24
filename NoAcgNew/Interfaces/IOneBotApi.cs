using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoAcgNew.Onebot.Models;

namespace NoAcgNew.Interfaces
{
	public interface IOneBotApi
	{
		public OneBotApiType GetApiType();
	}
}
