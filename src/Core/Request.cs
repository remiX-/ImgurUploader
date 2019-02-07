using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ImgurUploader
{
	public class Request
	{
		public static bool AnonState = false;

		internal const string CONTENT = "application/json";

		public static async Task<object> ExecuteAsync(string verb, string url, string obj)
		{
			var HttpRequest = CreateRequest(url, verb);

			if (obj != null)
			{
				WriteStream(ref HttpRequest, obj);
			}

			try
			{
				using (HttpWebResponse Response = (HttpWebResponse)(await HttpRequest.GetResponseAsync()))
				{
					return await ReadResponseAsync(Response);
				}
			}
			catch (WebException error)
			{
				return ReadResponseFromError(error);
			}
		}

		public static async Task<T> ExecuteAndDeserializeAsync<T>(string verb, string url, string obj)
		{
			object response = await ExecuteAsync(verb, url, obj);
			return await Task.Run(() => JsonConvert.DeserializeObject<T>(response.ToString()));
		}

		public static async Task<T> ExecuteAndDeserializeAsync<T>(string verb, string url)
		{
			return await ExecuteAndDeserializeAsync<T>(verb, url, null);
		}

		internal static HttpWebRequest CreateRequest(string URL, string Verb)
		{
			var basicRequest = (HttpWebRequest)WebRequest.Create(URL);
			basicRequest.ContentType = CONTENT;
			basicRequest.Method = Verb;
			if (!AnonState && AppGlobal.IsAuth)
				basicRequest.Headers.Add("Authorization", "Bearer " + AppGlobal.access_token);
			else
				basicRequest.Headers.Add("Authorization", "Client-ID " + AppGlobal.client_id);

			return basicRequest;
		}

		internal static void WriteStream(ref HttpWebRequest HttpRequest, object obj)
		{
			using (var streamWriter = new StreamWriter(HttpRequest.GetRequestStream()))
			{
				if (obj is string)
					streamWriter.Write(obj);
				else
					streamWriter.Write(JsonConvert.SerializeObject(obj));
			}
		}

		internal static string ReadResponse(HttpWebResponse HttpResponse)
		{
			if (HttpResponse != null)
			{
				using (var streamReader = new StreamReader(HttpResponse.GetResponseStream()))
				{
					return streamReader.ReadToEnd();
				}
			}

			return string.Empty;
		}

		internal static async Task<string> ReadResponseAsync(HttpWebResponse HttpResponse)
		{
			if (HttpResponse != null)
			{
				using (var streamReader = new StreamReader(HttpResponse.GetResponseStream()))
				{
					return await streamReader.ReadToEndAsync();
				}
			}

			return string.Empty;
		}

		internal static string ReadResponseFromError(WebException error)
		{
			using (var streamReader = new StreamReader(error.Response.GetResponseStream()))
			{
				return streamReader.ReadToEnd();
			}
		}
	}
}