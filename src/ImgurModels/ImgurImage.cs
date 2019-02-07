using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ImgurUploader
{
	public class ImgurImage : LocalImage
	{
		#region Vars
		string id;
		string deletehash;
		string name;
		string link;

		string type;
		int datetime;
		int width;
		int height;
		int size;
		int views;

		string account_id;
		string account_url;

		DateTime uploadedDateTime;

		public string Id
		{
			get { return id; }
			set { SetProperty(ref id, value); }
		}
		public string DeleteHash
		{
			get { return deletehash; }
			set { SetProperty(ref deletehash, value); }
		}
		public string Name
		{
			get { return name; }
			set { SetProperty(ref name, value); }
		}
		public string Link
		{
			get { return link; }
			set { SetProperty(ref link, value); }
		}

		public string Type
		{
			get { return type; }
			set { SetProperty(ref type, value); }
		}
		public int Datetime
		{
			get { return datetime; }
			set { SetProperty(ref datetime, value); }
		}
		public int Width
		{
			get { return width; }
			set { SetProperty(ref width, value); }
		}
		public int Height
		{
			get { return height; }
			set { SetProperty(ref height, value); }
		}
		public int Size
		{
			get { return size; }
			set { SetProperty(ref size, value); }
		}
		public int Views
		{
			get { return views; }
			set { SetProperty(ref views, value); }
		}

		public string Account_id
		{
			get { return account_id; }
			set { SetProperty(ref account_id, value); }
		}
		public string Account_url
		{
			get { return account_url; }
			set { SetProperty(ref account_url, value); }
		}

		public DateTime UploadedDateTime
		{
			get { return uploadedDateTime; }
			set { SetProperty(ref uploadedDateTime, value); }
		}
		#endregion

		public ImgurImage(string localPath) : base(localPath)
		{

		}

		public ImgurImage() : base(null) { }

		public void Init()
		{
			UploadedDateTime = Helpers.EpochToDateTime(Datetime);
		}
	}
}
