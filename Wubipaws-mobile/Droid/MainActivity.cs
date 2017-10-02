using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Platform.Mvvm;
using XLabs.Forms;
using System.Linq;
using XLabs.Serialization;
using System.Threading;
using Xamarin.Forms;
using System.IO;
using XLabs.Caching;
using XLabs.Caching.SQLite;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms.Controls;
using Android.Graphics;

namespace Wubipaws.Mobile.Droid
{
	

	[Activity (Label = "Wubi Paws", Icon = "@drawable/icon", 
		MainLauncher = true, 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : XFormsApplicationDroid //global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
		
			base.OnCreate (savedInstanceState);

			global::Xamarin.Forms.Forms.Init (this, savedInstanceState);
			SetIoc ();
				
			LoadApplication (new App ());
		}

		/// <summary>
		/// Sets the IoC.
		/// </summary>
		private void SetIoc ()
		{
			var resolverContainer = new SimpleContainer ();
			var app = new XLabs.Forms.XFormsAppDroid (this);
			var documents = app.AppDataDirectory;
			var appSettings = new AppSettings ();

			resolverContainer.Register<IDevice> (t => global::XLabs.Platform.Device.AndroidDevice.CurrentDevice);
			resolverContainer.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			resolverContainer.Register<INetwork> (t => t.Resolve<IDevice> ().Network);
			resolverContainer.Register<IApiServices> (t => new ApiServices ());
			resolverContainer.Register<XLabs.Platform.Mvvm.IXFormsApp> ((IXFormsApp)app)
				.Register<IDependencyContainer> (t => resolverContainer);
			// register ISimpleCache to Func<IResolver, ISimpleCache> 
			// returning a new instance of SQLiteSimpleCache
			// with one of the constructor parameters being resolved by 
			// IResolver to previously registered JSON serializer
			string pathToDatabase = System.IO.Path.Combine (documents, "wubipaws.db");
			resolverContainer.Register<ISimpleCache> (
				t => (XLabs.Caching.ISimpleCache)new SQLiteSimpleCache (new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid (),
					new SQLite.Net.SQLiteConnectionString (pathToDatabase, true), t.Resolve<IJsonSerializer> ()));
			
			resolverContainer.Register<IJsonSerializer> (t => new XLabs.Serialization.JsonNET.JsonSerializer ());
			resolverContainer.Register<IProgressDialog> (new ProgressDialog ());

			resolverContainer.Register<IAppSettings> (t =>
			{
				
				appSettings.ApiServiceHost = "https://wubipaws.azurewebsites.net/";
				appSettings.WebPortalHost = "https://wubipaws-portal.azurewebsites.net/";
				appSettings.AppBundleResourcePath = "file:///android_asset/";
				appSettings.AppDataDirectory = app.AppDataDirectory;
				return appSettings;
			});
		
			Resolver.SetResolver (resolverContainer.GetResolver ());

		}
	}




}

