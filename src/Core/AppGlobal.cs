using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurUploader
{
    public static class AppGlobal
    {
		public static bool IsAuth { get; set; } = false;
		public static string client_id { get; set; } = "423439a3431ac7e";
		public static string client_secret { get; set; } = "01b01bc1e54315297f402d0a657a0d0fb2eb9f3a";
		public static string access_token { get; set; }
		public static string refresh_token { get; set; }
	}
}
