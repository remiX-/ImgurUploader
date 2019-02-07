using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurUploader
{
    public class ImgurAuth
    {
		public string access_token { get; set; }
		public string refresh_token { get; set; }

		public int account_id { get; set; }
		public string account_username { get; set; }

		public string token_type { get; set; }
		public int expires_in { get; set; }
    }
}
