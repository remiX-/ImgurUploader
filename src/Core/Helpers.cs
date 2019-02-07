using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurUploader
{
	public static class Helpers
	{
		#region DateTime Methods
		public static int GetEpochTime()
		{
			return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static DateTime EpochToDateTime(int epoch)
		{
			return new DateTime(1970, 1, 1).AddSeconds(epoch);
		}
		#endregion
	}
}
