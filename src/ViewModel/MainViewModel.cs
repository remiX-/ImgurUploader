using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ImgurUploader
{
	public class MainViewModel : BindableBase
	{
		#region Variables
		public ObservableCollection<LocalImage> Uploads { get; set; } = new ObservableCollection<LocalImage>();
		public ObservableCollection<ImgurImage> CompleteUploads { get; set; } = new ObservableCollection<ImgurImage>();

		string accountName;
		string accountPin;

		string status;
		Brush statusColour;
		string requestText;

		public string AccountName { get { return accountName; } set { SetProperty(ref accountName, value); } }
		public string AccountPin { get { return accountPin; } set { SetProperty(ref accountPin, value); } }

		public string Status { get { return status; } set { SetProperty(ref status, value); } }
		public Brush StatusColour { get { return statusColour; } set { SetProperty(ref statusColour, value); } }
		public string RequestText { get { return requestText; } set { SetProperty(ref requestText, value); } }
		#endregion

		public MainViewModel()
		{
			AccountName = "Disconnected";

			Status = "Press Request Auth to connect your account";
			StatusColour = Brushes.Yellow;
			RequestText = "Request Auth";
		}

		public void AddItemsToView(List<string> files)
		{
			Uploads.AddRange(files.Select(x => new LocalImage(x)));
		}

		public bool RemoveItemFromView(LocalImage item)
		{
			return Uploads.Remove(item);
		}

		public bool HasFileInView(string filePath)
		{
			return Uploads.FirstOrDefault(x => x.LocalPath == filePath) != null;
		}

		public bool UpdateUIForComplete(LocalImage image, ImgurImage image2)
		{
			if (!Uploads.Remove(image))
				return false;

			CompleteUploads.Add(image2);

			return true;
		}
	}
}