using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Security.Policy;
using System.IO;
using Xamarin.Forms.Platform;
using XLabs.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using System.Net;
using XLabs.Serialization;
using Newtonsoft.Json;
using BigTed;
using XLabs.Caching;
using XLabs.Caching.SQLite;

namespace Wubipaws.Mobile.iOS
{

	[Register ("AppDelegate")]
	public partial class AppDelegate :  XFormsApplicationDelegate//global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool WillFinishLaunching (UIApplication uiApplication, NSDictionary launchOptions)
		{
		 
			uiApplication.StatusBarHidden = true;
			return base.WillFinishLaunching (uiApplication, launchOptions);
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			SetIoc ();
			app.StatusBarHidden = true;
			LoadApplication (new Wubipaws.Mobile.App ());

			return base.FinishedLaunching (app, options);
		}

		/// <summary>
		/// Sets the IoC.
		/// </summary>
		private void SetIoc ()
		{
			var resolverContainer = new SimpleContainer ();
			var app = new XLabs.Forms.XFormsAppiOS (this);
			var documents = app.AppDataDirectory;
			var appSettings = new AppSettings ();
		
			resolverContainer.Register<IDevice> (t => global::XLabs.Platform.Device.AppleDevice.CurrentDevice);
			resolverContainer.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			resolverContainer.Register<INetwork> (t => t.Resolve<IDevice> ().Network);
			resolverContainer.Register<IApiServices> (t => new ApiServices ());
			resolverContainer.Register<XLabs.Platform.Mvvm.IXFormsApp> ((IXFormsApp)app)
							.Register<IDependencyContainer> (t => resolverContainer);
			// register ISimpleCache to Func<IResolver, ISimpleCache> 
			// returning a new instance of SQLiteSimpleCache
			// with one of the constructor parameters being resolved by 
			// IResolver to previously registered JSON serializer
			string pathToDatabase = Path.Combine (documents, "wubipaws.db");
			resolverContainer.Register<ISimpleCache> (
				t => (XLabs.Caching.ISimpleCache)new SQLiteSimpleCache (new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS (),
					new SQLite.Net.SQLiteConnectionString (pathToDatabase, true), t.Resolve<IJsonSerializer> ()));

			resolverContainer.Register<IProgressDialog> (new ProgressDialog ());
		
			resolverContainer.Register<IJsonSerializer> (t => new XLabs.Serialization.JsonNET.JsonSerializer ());
			resolverContainer.Register<IAppSettings> (t =>
			{

		
				appSettings.AppBundleResourcePath = NSBundle.MainBundle.BundlePath + "/";
				appSettings.AppDataDirectory = app.AppDataDirectory;
				appSettings.WebPortalHost = "https://wubipaws-portal.azurewebsites.net/";
				appSettings.ApiServiceHost = "https://wubipaws.azurewebsites.net/";
			
				return appSettings;
			});
			
			Resolver.SetResolver (resolverContainer.GetResolver ());

		}

	}
}

