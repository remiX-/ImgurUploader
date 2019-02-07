using Prism.Mvvm;
using System.IO;

namespace ImgurUploader
{
	public class LocalImage : BindableBase
	{
		#region Vars
		string localPath;
		string fileName;
		long fileSize;

		public string LocalPath
		{
			get { return localPath; }
			set
			{
				SetProperty(ref localPath, value);

				FileInfo fi = new FileInfo(LocalPath);
				if (fi.Exists)
				{
					fileName = fi.Name;
					fileSize = fi.Length / 1000;
				}
			}
		}
		public string FileName
		{
			get { return fileName; }
			set { SetProperty(ref fileName, value); }
		}
		public long FileSize
		{
			get { return fileSize; }
			set { SetProperty(ref fileSize, value); }
		}
		#endregion

		public LocalImage(string localPath)
		{
			if (!string.IsNullOrWhiteSpace(localPath))
			{
				LocalPath = localPath;
			}
		}	}
}
