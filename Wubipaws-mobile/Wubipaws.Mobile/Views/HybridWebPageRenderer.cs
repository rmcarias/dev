using System;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using XLabs.Ioc;
using System.Collections.Generic;

namespace Wubipaws.Mobile
{
	public class HybridWebPageRenderer : ContentPage
	{

		private HybridWebPageView hyrbidPage = null;

		public HybridWebPageRenderer (HybridWebViewModel viewModel, object razorPageInstanceOrUrl)
		{
			hyrbidPage = new HybridWebPageView (viewModel, razorPageInstanceOrUrl);
			this.Content = hyrbidPage;
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			if (hyrbidPage.ViewHasErrors)
			{
				await DisplayAlert ("Error Loading", "An unexpected error showing this page has occurred.", "OK");
				await Navigation.PopModalAsync ();
			}
		}
	}

	public class HybridWebPageView:ContentView
	{

		public HybridWebPageView (HybridWebViewModel viewModel, object razorPageInstanceOrUrl)
		{
			if (razorPageInstanceOrUrl == null)
				throw new ArgumentNullException ("razorPageInstance", "An instance of a Razor Page is needed");

			var appSettings = Resolver.Resolve<IAppSettings> ();
			var content = string.Empty;
			var isWebPortalRequest = false;
			var loader = Resolver.Resolve<IProgressDialog> ();
			loader.Title = "Loading Page...";
			loader.Show ();
			try
			{
				var pageType = razorPageInstanceOrUrl.GetType ();
				if (pageType != typeof(System.String))
				{
					pageType.GetRuntimeProperty ("Model").SetValue (razorPageInstanceOrUrl, viewModel);
					MethodInfo info = pageType.GetRuntimeMethod ("GenerateString", new Type[]{ });
					content = (string)info.Invoke (razorPageInstanceOrUrl, null);
				} else
				{

					content = razorPageInstanceOrUrl.ToString ();
					isWebPortalRequest = true;
				}
			} catch
			{
				ViewHasErrors = true;
				loader.Hide ();
				return;
			}
			var stack = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			var hwv = new HybridWebView {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			hwv.LoadFinished += (object sender, EventArgs e) =>
			{
				loader.Hide ();
			};

			if (viewModel.NativeCallbacks.Count > 0)
			{
				foreach (KeyValuePair<string,Action<string>> pair in viewModel.NativeCallbacks)
				{
					hwv.RegisterCallback (pair.Key, t => Device.BeginInvokeOnMainThread (() =>
					{
						pair.Value (t);	
					}));

				}
			}

			if (viewModel.ShowHeader)
			{
				var header = new FormHeaderBar ();
				header.Text = viewModel.Title;
				header.HasCloseButton = true;
				header.CloseButtonClicked += async (object sender, EventArgs e) =>
				{
					await Navigation.PopModalAsync ();
				};
				stack.Children.Add (header);
			}
			stack.Children.Add (hwv);
			this.Content = stack;


			if (!isWebPortalRequest)
			{
				hwv.Source = new HtmlWebViewSource (){ Html = content };
			} else
			{

				var uri = new Uri (string.Format ("{0}{1}", appSettings.WebPortalHost, content));
				var qsColl = HttpUtility.ParseQueryString (uri.Query);
				qsColl.Add ("mobilerequest_auth_token", App.MobileSession.AuthToken);
				var qs = HttpUtility.GenerateQueryString (qsColl);
				var request = new UriBuilder (uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath, qs).Uri;

				#if DEBUG
				Debug.WriteLine ("Hybrid Request Uri:" + request.ToString ());
				#endif
				hwv.Uri = request;

			}

		}

		public bool ViewHasErrors
		{
			get;
			private set;
		}

	}
}

