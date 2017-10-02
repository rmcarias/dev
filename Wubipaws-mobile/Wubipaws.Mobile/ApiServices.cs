using System;
using System.Threading.Tasks;
using ModernHttpClient;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using XLabs.Ioc;
using WubiPaws.DTO;

namespace Wubipaws.Mobile
{

	public interface IApiServices
	{
		string BaseApiUrl
		{
			get;
		}

		HttpClient CreateServiceClient ();

		Task<ApiResponse> PostAsync (string resourcePath, object postData);

		Task<ApiResponse> GetAsync (string resourcePath, object postData = null);

		JObject SerializeToJObject (object o);

		Task<bool> ApiIsRunning ();

	}

	public class ApiServices : IApiServices
	{
		static IAppSettings AppSettings;

		static ApiServices ()
		{
			
			JsonConvert.DefaultSettings = () =>
			{
				return GetDefaultJsonSerializerSettings ();
			
			};

			AppSettings = Resolver.Resolve<IAppSettings> ();
		}

		private static JsonSerializerSettings GetDefaultJsonSerializerSettings ()
		{
			JsonSerializerSettings jsonSetting = new JsonSerializerSettings ();
			jsonSetting.Converters.Add (new Newtonsoft.Json.Converters.StringEnumConverter ());
			jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver ();
			jsonSetting.DateFormatString = "s";
			jsonSetting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			jsonSetting.DateParseHandling = DateParseHandling.DateTime;
			return jsonSetting;
		}

		public ApiServices ()
		{
		}

		public string BaseApiUrl
		{
			get { 

				if (AppSettings.ApiServiceHost.Contains (".azurewebsites."))
				{
					return AppSettings.ApiServiceHost + "api/";
				}

				return  AppSettings.ApiServiceHost + "wubipaws.api/api/";

			}
		}


		public   HttpClient CreateServiceClient ()
		{

			var restClient = new HttpClient (new NativeMessageHandler ());
			restClient.BaseAddress = new Uri (BaseApiUrl);
			restClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
			restClient.DefaultRequestHeaders.TryAddWithoutValidation ("X-AppContext", "mobileapp");
			restClient.DefaultRequestHeaders.TryAddWithoutValidation ("X-RequestVerificationToken", AppSettings.RequestVerificationToken);

			if (App.MobileSession.IsAuthorized)
			{
				restClient.DefaultRequestHeaders.TryAddWithoutValidation ("From", App.MobileSession.UserName);
				restClient.DefaultRequestHeaders.TryAddWithoutValidation ("Authorization", "Bearer " + App.MobileSession.AuthToken);
			}
			return restClient;

		}


		public async Task<ApiResponse> PostAsync (string resourcePath, object postData)
		{
			var response = new ApiResponse ();

			var jsonPostData = postData != null && postData.GetType () == typeof(JObject) ? (JObject)postData :
				SerializeToJObject (postData);
			var content = ConvertToStringContent (jsonPostData);

			#if DEBUG
			if (jsonPostData != null)
			{
				Debug.WriteLine ("POST=>" + jsonPostData.ToString (Formatting.None));
			}
			#endif

			response = await ExecuteAsync (HttpMethod.Post, resourcePath, content);
			return response;
		}

		public async Task<ApiResponse> GetAsync (string resourcePath, object postData = null)
		{			
			SerializeToJObject (postData);

//			#if DEBUG
//			if(jsonPostData != null){
//				Debug.WriteLine("GET=>" + jsonPostData.ToString (Formatting.None));
//
//			}
//			#endif
			var response = await ExecuteAsync (HttpMethod.Get, resourcePath, null);
			return response;
		}


		public async Task<bool> ApiIsRunning ()
		{
			try
			{
				await GetAsync ("app/wubi/status");
				return true;

			} catch (HttpRequestException)
			{

			}
			return false;	
		}


		public JObject SerializeToJObject (object o)
		{
			if (o == null)
				return null;

			var j = new JsonSerializer ();
			var ser =	GetDefaultJsonSerializerSettings ();
			j.DateFormatHandling = ser.DateFormatHandling;
			j.DateFormatString = ser.DateFormatString;
			j.DateParseHandling = ser.DateParseHandling;
			j.ContractResolver = new CamelCasePropertyNamesContractResolver ();
			j.Converters.Add (new StringEnumConverter ());

			return	JObject.FromObject (o, j);

		}

		#region Private Methods

		private HttpContent ConvertToStringContent (JObject postData)
		{
			StringContent content = null;
			if (postData != null)
			{

				var stringData = postData.ToString (Formatting.None);
				content = new StringContent (stringData, Encoding.UTF8, "application/json");

			}
			return content;
		}

		private async Task<ApiResponse> ExecuteAsync (HttpMethod method, string resourcePath, HttpContent content = null)
		{
			HttpResponseMessage responseMessage = new HttpResponseMessage ();
			JObject data = new JObject ();
			var apiResponse = new ApiResponse () { 
				Data = data,
				IsUnAuthorizedError = false,
				IsSuccessStatusCode = false,
				StatusCode = 200
			};
			var package = 
				content != null ? new HttpRequestMessage (method, resourcePath) { Content = content } :
				new HttpRequestMessage (method, resourcePath);

			try
			{
				using (var client = CreateServiceClient ())
				{
					responseMessage =	await client.SendAsync (package);
				}

			} catch (Exception ex)
			{
				#if DEBUG
				Debug.WriteLine ("Request Uri: " + package.RequestUri.ToString ());
				Debug.WriteLine ("Error ExecuteAsync: " + ex.Message);
				#endif
				return apiResponse;
			}


			if (!responseMessage.IsSuccessStatusCode)
			{
				string message = await responseMessage.Content.ReadAsStringAsync ();
				string err = "Response Status:" + responseMessage.ReasonPhrase + ",Message:" + message;
				#if DEBUG
				Debug.WriteLine (err);
				Debug.WriteLine ("Request Uri: " + package.RequestUri.ToString ());
				#endif
				apiResponse.ErrorMessage = err;
				if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					apiResponse.IsUnAuthorizedError = true;
				}
				apiResponse.StatusCode = (int)responseMessage.StatusCode;
				return apiResponse;
			}

			string jsonData = string.Empty;
			try
			{

				jsonData = await responseMessage.Content.ReadAsStringAsync ();
				if (!string.IsNullOrEmpty (jsonData))
				{
					//Try parsing as ApiResponse first
					apiResponse = JsonConvert.DeserializeObject<ApiResponse> (jsonData);
					if (apiResponse.Data == null)
					{
						if (!ObjectExtensions.JObjectTryParse (jsonData, out data) || data == null)
						{
							data = JObject.Parse (jsonData);
						}

					} else
					{
						data = apiResponse.Data;	
					}

				}

			} catch (Exception)
			{
				data = JObject.FromObject (new { ResponseMessage = jsonData });
			}

			#if DEBUG
			Debug.WriteLine (data.ToString (Formatting.Indented));
			#endif
			return new ApiResponse () { Data = data, IsSuccessStatusCode = true  };
		}

		#endregion
	}



}

