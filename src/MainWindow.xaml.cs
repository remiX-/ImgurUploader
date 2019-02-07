using MahApps.Metro;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ImgurUploader
{
	public partial class MainWindow : MetroWindow
	{
		#region Variables
		private MainViewModel MyViewModel;

		private ImgurAPI Imgur { get; }
		
		private string[,] imageTypes = new string[,]
		{
			{ "PNG", "*.png" },
			{ "JPEG", "*.jpeg;*.jpg" },
			{ "BMP", "*.bmp" },
			{ "GIF", "*.gif" },
			{ "TIFF", "*.tiff;*.tif" },
		};
		private string imageFilter = "ALL|";
		#endregion

		#region Window Events
		public MainWindow()
		{
			InitializeComponent();

			Imgur = new ImgurAPI(AppGlobal.client_id, AppGlobal.client_secret);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//DataContext = MyViewModel = new MainViewModel();
			MyViewModel = DataContext as MainViewModel;

			for (int i = 0; i < imageTypes.GetLength(0); i++)
				imageFilter += imageTypes[i, 1] + ";";
			imageFilter = imageFilter.Substring(0, imageFilter.Length - 1) + "|";

			for (int i = 0; i < imageTypes.GetLength(0); i++)
				imageFilter += imageTypes[i, 0] + "|" + imageTypes[i, 1] + "|";
			imageFilter = imageFilter.Substring(0, imageFilter.Length - 1);

			if (!string.IsNullOrEmpty(Properties.Settings.Default.Account_RefreshToken))
			{
				AppGlobal.IsAuth = true;
				AppGlobal.access_token = Properties.Settings.Default.Account_AccessToken;
				AppGlobal.refresh_token = Properties.Settings.Default.Account_RefreshToken;

				MyViewModel.AccountName = Properties.Settings.Default.Account_Username;
				txt_Pin.IsEnabled = false;

				MyViewModel.Status = "Success";
				MyViewModel.StatusColour = Brushes.Green;
			}

			//Properties.Settings.Default.Account_Username = string.Empty;
			//Properties.Settings.Default.Account_AccessToken = string.Empty;
			//Properties.Settings.Default.Account_RefreshToken = string.Empty;
			//Properties.Settings.Default.Save();
		}
		#endregion

		#region Auth Account
		private async void btn_Request_Click(object sender, RoutedEventArgs e)
		{
			if (AppGlobal.IsAuth)
			{
				AppGlobal.IsAuth = false;
				AppGlobal.access_token = string.Empty;

				Properties.Settings.Default.Account_Username = string.Empty;
				Properties.Settings.Default.Account_AccessToken = string.Empty;
				Properties.Settings.Default.Account_RefreshToken = string.Empty;
				Properties.Settings.Default.Save();

				MyViewModel.AccountName = string.Empty;

				MyViewModel.Status = "Press Request Auth to connect your account";
				MyViewModel.StatusColour = Brushes.Yellow;
				MyViewModel.RequestText = "Request Auth";

				return;
			}

			if (!txt_Pin.IsEnabled)
			{
				GoToSite(Imgur.GetAuthLink("pin"));

				MyViewModel.Status = "Enter PIN and press Authorize to connect your account";
				MyViewModel.StatusColour = Brushes.Orange;
				MyViewModel.RequestText = "Authorize";

				txt_Pin.IsEnabled = true;
			}
			else
			{
				string pin = txt_Pin.Text.Trim();
				if (pin.Length == 0) return;

				ImgurAuth imgur_data = await Imgur.GetTokenByPin(pin);
				if (imgur_data != null)
				{
					AppGlobal.IsAuth = true;
					AppGlobal.access_token = imgur_data.access_token;

					Properties.Settings.Default.Account_Username = imgur_data.account_username;
					Properties.Settings.Default.Account_AccessToken = imgur_data.access_token;
					Properties.Settings.Default.Account_RefreshToken = imgur_data.refresh_token;
					Properties.Settings.Default.Save();

					MyViewModel.AccountName = imgur_data.account_username;

					txt_Pin.IsEnabled = false;

					MyViewModel.Status = "Success";
					MyViewModel.StatusColour = Brushes.Green;
					MyViewModel.RequestText = "De-Authorize";
				}
			}
		}
		#endregion

		#region Drag and drop
		private void Window_Drop(object sender, DragEventArgs e)
		{
			Activate();

			var allFiles = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
			AddFilesToView(allFiles);
		}

		private void Window_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effects = DragDropEffects.Copy;
		}
		#endregion

		#region Misc
		private void GoToSite(string url)
		{
			Process.Start(url);
		}

		private void AddFilesToView(List<string> files)
		{
			List<string> validFiles = new List<string>();

			foreach (string path in files)
			{
				if (string.IsNullOrEmpty(path)) continue;
				if (!PathHasValidExtension(path)) continue;
				if (MyViewModel.HasFileInView(path)) continue;

				validFiles.Add(path);
			}

			MyViewModel.AddItemsToView(validFiles);

			btn_Upload.IsEnabled = true;
		}

		private bool PathHasValidExtension(string path)
		{
			return imageFilter.Contains(Path.GetExtension(path).ToLower());
		}
		#endregion

		private void btn_SelectImages_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog _openFileDialog = new OpenFileDialog
			{
				Title = "Select an image to upload",
				Filter = imageFilter,
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				Multiselect = true
			};

			if ((bool)_openFileDialog.ShowDialog())
			{
				var allFiles = _openFileDialog.FileNames.ToList();

				AddFilesToView(allFiles);
			}
		}

		private async void btn_Upload_Click(object sender, RoutedEventArgs e)
		{
			btn_SelectImages.IsEnabled = false;
			btn_Upload.IsEnabled = false;

			Request.AnonState = (bool)cb_Anon.IsChecked;

			if (!AppGlobal.IsAuth)
				Request.AnonState = true;

			var uploads = MyViewModel.Uploads.ToList();
			int count = 0;
			foreach (LocalImage image in uploads)
			{
				lbl_Status.Content = string.Format("Uploading image {0}/{1}", ++count, uploads.Count);
				ImgurWrapper result = await Imgur.UploadImage(image.LocalPath);

				if (result.success)
				{
					ImgurImage uploaded = JsonConvert.DeserializeObject<ImgurImage>(result.data.ToString());
					uploaded.LocalPath = image.LocalPath;
					uploaded.Init();

					MyViewModel.UpdateUIForComplete(image, uploaded);

					GoToSite(uploaded.Link);
				}

				pb_Progress.Value = ((double)count / uploads.Count) * 100;
			}

			btn_SelectImages.IsEnabled = true;
		}

		#region DataGrid
		private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
		{
			var selected = DG_ToUpload.SelectedItems.Cast<LocalImage>().ToList();
			foreach (var item in selected)
			{
				MyViewModel.RemoveItemFromView(item);
			}
		}
		#endregion

		#region Settings
		private void cb_Anon_Checked(object sender, RoutedEventArgs e)
		{
			//if ((bool)cb_Anon.IsChecked)
			//{
			//	btn_Request.IsEnabled = false;
			//	btn_Request.Content = "Request Authorization";

			//	txt_Account.Text = "Anonymous";
			//	txt_Pin.Text = string.Empty;
			//	txt_Pin.IsEnabled = false;
			//}
			//else
			//{
			//	btn_Request.IsEnabled = true;
			//	txt_Pin.IsEnabled = true;
			//	txt_Account.Text = "";
			//}
		}

		private void cb_DarkTheme_Checked(object sender, RoutedEventArgs e)
		{
			var theme = ThemeManager.DetectAppStyle();

			ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetInverseAppTheme(theme.Item1));
		}
		#endregion
	}
}
