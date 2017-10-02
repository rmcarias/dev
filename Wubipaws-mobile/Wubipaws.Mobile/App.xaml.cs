using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Mvvm;
using System.Dynamic;
using XLabs.Forms.Services;
using XLabs.Platform.Services;
using XLabs.Caching;
using System.Diagnostics;
using XLabs.Forms.Controls;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace Wubipaws.Mobile
{
	
	public partial class App : Application
	{
		private NavigationView navView;

		public App ()
		{
			InitializeComponent ();
			Style s = new Style (typeof(ExtendedEntry));
			s.Setters.Add (new Setter () {
				Property = Entry.TextColorProperty,
				Value = Color.FromHex ("#212121")

			});

			if (Device.OS == TargetPlatform.Android)
			{
				s.Setters.Add (new Setter () {
					Property = ExtendedEntry.PlaceholderTextColorProperty,
					Value = Color.FromHex ("#CCCCCC")

				});
			}

			this.Resources.Add (s);

			Init ();
			DetermineAppStartup ();
			// The root page of your application
			MainPage = navView;

		}

		private void Init ()
		{

			var app = Resolver.Resolve<IXFormsApp> ();
			if (app == null)
			{
				return;
			}
			app.Error += (o, e) => Debug.WriteLine ("Application Error");

			ViewFactory.Register<LoginPage,LoginViewModel> ();
			ViewFactory.Register<RegisterPage,RegisterViewModel> ();
			ViewFactory.Register<AccountIndexPage,AccountViewModel> ();
			ViewFactory.Register<MyProfilePage,AccountViewModel> ();
			ViewFactory.Register<PetServicesListPage,PetServicesViewModel> ();
			ViewFactory.Register<PetsList,PetListingViewModel> ();
			ViewFactory.Register<PetProfilePage,PetProfileViewModel> ();
			ViewFactory.Register<PetGroomingProfilePage,PetGroomingProfileViewModel> ();
			ViewFactory.Register<PetGroomingConfirmationPage,PetGroomingConfirmationViewModel> ();
			Resolver.Resolve<IDependencyContainer> ().Register<IApiServices> (new ApiServices ());

		}

		private void DetermineAppStartup ()
		{
			Page page;		
			if (ShowSignInScreen ())
			{
				page = CreatePage (false);
			} else
			{
				page = CreatePage (true);
			}
			SetNavService (page);
		}

		private void SetNavService (Page page)
		{
			navView = new NavigationView (page);
			NavigationService navService = new NavigationService (navView.Navigation);
			Resolver.Resolve<IDependencyContainer> ().Register<INavigationService> (navService);
		}

		private bool ShowSignInScreen ()
		{
			object appSettingsLaunchScreen = null;
			if (App.Current.Properties.TryGetValue (AppSettings.Constants.AppLaunchSettingsCacheKey, out appSettingsLaunchScreen))
			{
				var enu = (DefaultLandingScreen)appSettingsLaunchScreen;
				return enu == DefaultLandingScreen.SignIn;

			}
			return false;
		}

		private Page CreatePage (bool indexPage)
		{

			if (!indexPage)
			{
				return  ViewFactory.CreatePage<LoginViewModel,LoginPage> ((v, p) =>
				{
					v.IsDefaultPage = true;
				}) as LoginPage;
			}
			return new IndexPage ();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
			App.MobileSession = new MobileAuthSession (string.Empty, 
				DateTime.MinValue.ToUniversalTime ().ToString (),
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty);
			
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
			bool resetPage = false;
		
			if ((MainPage as NavigationPage).CurrentPage.GetType () == typeof(LoginPage))
			{
				if (ShowSignInScreen ())
				{
					return;
				} 
				resetPage = true;

			} else if ((MainPage as NavigationPage).CurrentPage.GetType () == typeof(IndexPage))
			{
				if (!ShowSignInScreen ())
				{
					return;
				}
				resetPage = true;

			}
			if (resetPage)
			{
				SetNavService (CreatePage (!ShowSignInScreen ()));
				MainPage = navView;
				resetPage = false;
			}

		}

		#region Global Static Props

		public static IMobileAuthSession MobileSession
		{
			get;
			set;

		}

		public static IProgressDialog GetLoadingDialog (string title = "")
		{
			var dlg = Resolver.Resolve<IProgressDialog> ();
			dlg.Title = string.IsNullOrEmpty (title) ? AppSettings.Constants.LoaderText : title;
			dlg.IsDeterministic = false;
			return dlg;
		}

		#endregion
	}
}

