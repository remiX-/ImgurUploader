using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImgurUploader
{
	public class ImgurAPI
	{
		private string ClientId { get; }
		private string ClientSecret { get; }

		private string AppState { get; } = "olo";

		public ImgurAPI(string clientId, string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
		}

		public string GetAuthLink(string responseType)
		{
			return string.Format("https://api.imgur.com/oauth2/authorize?client_id={0}&response_type={1}&state={2}", ClientId, responseType, AppState);
		}

		public async Task<ImgurWrapper> UploadImage(string imagePath)
		{
			JObject jObject = new JObject
			{
				["image"] = Convert.ToBase64String(File.ReadAllBytes(imagePath)),
			};

			return await Request.ExecuteAndDeserializeAsync<ImgurWrapper>("POST", "https://api.imgur.com/3/image/", jObject.ToString());
		}

		public async Task<ImgurAuth> GetTokenByPin(string pin)
		{
			JObject jObject = new JObject
			{
				["client_id"] = AppGlobal.client_id,
				["client_secret"] = AppGlobal.client_secret,
				["grant_type"] = "pin",
				["pin"] = pin
			};

			return await Request.ExecuteAndDeserializeAsync<ImgurAuth>("POST", "https://api.imgur.com/oauth2/token/", jObject.ToString());
		}

		public async Task<ImgurAuth> GetTokenByRefresh(string refreshToken)
		{
			JObject jObject = new JObject
			{
				["client_id"] = AppGlobal.client_id,
				["client_secret"] = AppGlobal.client_secret,
				["grant_type"] = "refresh_token",
				["refresh_token"] = refreshToken
			};

			return await Request.ExecuteAndDeserializeAsync<ImgurAuth>("POST", "https://api.imgur.com/oauth2/token/", jObject.ToString());
		}
	}
}
