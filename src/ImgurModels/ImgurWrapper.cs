using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgurUploader
{
    public class ImgurWrapper
    {
		public object data { get; set; }
		public bool success { get; set; }
		public int status { get; set; }

		public ImgurImage Image { get; set; }

		public ImgurWrapper()
		{

		}

		//public void Init()
		//{
		//	Image = JsonConvert.DeserializeObject<ImgurImage>(data.ToString());
		//}
	}
}
