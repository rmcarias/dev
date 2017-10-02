using System;
using Xamarin.Forms;

namespace Wubipaws.Mobile
{
	public enum DefaultLandingScreen
	{
		Home = 1,
		SignIn = 2
	}


	public interface IAppSettings
	{
		string ApiServiceHost
		{
			get;
			set;
		}

		/// <summary>
		/// Points to Device specific data personal directory
		/// </summary>
		/// <value>The app data directory.</value>
		string AppDataDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Points to Device specific bundled resource directory
		/// </summary>
		/// <value>The app bundle resource path.</value>
		string AppBundleResourcePath
		{
			get;
			set;
		}

		string RequestVerificationToken
		{
			get;

		}

		string WebPortalHost
		{
			get;
			set;
		}
	}

	public class AppSettings : IAppSettings
	{
		public AppSettings ()
		{
			this.requestVerificationToken = "1000:3kLqjsDOzjkLlnshL8CQaJHj3NAeC6zc:td5Fu5FpAidkNVi2sW3wGNHK9JZa4R32";

		}

		public string ApiServiceHost
		{
			get;
			set;
		}

		public string AppDataDirectory
		{
			get;
			set;
		}

		public string AppBundleResourcePath
		{
			get;
			set;
		}

		private string requestVerificationToken;

		public string RequestVerificationToken
		{
			get{ return this.requestVerificationToken; }

		}

		public string WebPortalHost
		{
			get;
			set;
		}



		public static class Constants
		{
			public static string DisplayPageErrorMessage = "There was an error loading the page.";
			public static string DisplayGeneralErrorDlgTitle = "Error";
			public static string DisplayGeneralErrorDlgMessage = "There was an error with your request. Please try again.";
			public static string FormErrorDlgTitle = "Form Error";
			public static string FormErrorDlgMessage = "There are errors with the form.";


			public static string SaveLoaderText = "Saving...";
			public static string LoaderText = "Loading...";
			public static string SchedulingGroomingLoaderText = "Saving...";

			public static string SaveSuccessDlgTitle = "Save Success";
			public static string SaveSuccessDlgMessage = "Changes have been saved.";

			public static string SaveErrorDlgTitle = "Save Error";
			public static string SaveErrorDlgMessage = "There was an error saving your changes. Please try again.";

			public static string AppLaunchSettingsCacheKey = "WubiPawsApp:AppLaunchSettings";
			public static string AppPetProfileCacheKey = "WubiPawsApp:AppPetProfile";
			public static string GroomingAppointmentsCacheKey = "WubiPawsApp:GroomingAppointments";

			public static Color RubyTintColor = Color.FromHex ("#E91E63");
			public static Color LightGrayTintColor = Color.FromHex ("#FAFAFA");
			public static Color DarkGrayBackgroundTintColor = Color.FromHex ("#37474F");
			public static Color ButtonBackgroundColor = Color.FromHex ("#ECF0F1");
			public static Color ButtonPrimaryGreenTintColor = Color.FromHex ("#00B210");
			public static Color ButtonSecondaryBlueTintColor = Color.FromHex ("#098FB2");
		}

	}
}

