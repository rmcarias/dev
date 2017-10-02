using System;

using Xamarin.Forms;
using System.Text;
using XLabs.Forms.Mvvm;
using Wubipaws.Mobile;
using System.Linq.Expressions;
using XLabs.Ioc;
using System.Diagnostics;
using XLabs.Forms.Controls.SensorBar;
using XLabs.Forms.Controls;

namespace Wubipaws.Mobile
{
	[ViewType (typeof(AccountViewModel))]
	public class AccountIndexPage : ExtendedTabbedPage
	{
		private bool showSignOutConfirmDlg = true;

		public AccountIndexPage ()
		{
			this.Title = "Wubi Paws"; //Bug should be My Account;

			ToolbarItem logout = new ToolbarItem ();
			logout.Order = ToolbarItemOrder.Primary;
			logout.Icon = (FileImageSource)FileImageSource.FromFile ("ic_exit_to_app_black_24dp.png");
			NavigationPage.SetHasBackButton (this, false);
			NavigationPage.SetBackButtonTitle (this, "Back");

			logout.Clicked += SignOut;
			this.CurrentPageChanged += AccountIndexPage_CurrentPageChanged;

			this.ToolbarItems.Add (logout);
			if (Device.OS == TargetPlatform.iOS)
			{
				this.TintColor = AppSettings.Constants.ButtonSecondaryBlueTintColor;
			}
			this.Children.Add (ViewFactory.CreatePage <AccountViewModel, MyProfilePage> () as MyProfilePage);
			this.Children.Add (ViewFactory.CreatePage <PetListingViewModel, PetsList> () as PetsList);
			this.Children.Add (ViewFactory.CreatePage <PetServicesViewModel, PetServicesListPage> () as PetServicesListPage);
			this.Children.Add (new SettingsPage ());
			base.SwipeEnabled = true;
		}

		void AccountIndexPage_CurrentPageChanged ()
		{
			ChildPageChanged (this, EventArgs.Empty);
		}

		private void ChildPageChanged (Object sender, EventArgs e)
		{
			if (App.MobileSession.IsAuthorized == false)
			{
				showSignOutConfirmDlg = false;
				SignOut (sender, e);
			}	
		}

		private async void SignOut (Object sender, EventArgs e)
		{

			var result = showSignOutConfirmDlg ? false : true;
			if (showSignOutConfirmDlg)
			{
				result = await DisplayAlert ("Sign Out", "Are you sure?", "Yes", "No");
			} 

			if (result)
			{
				using (var loader = Resolver.Resolve<IProgressDialog> ())
				{
					loader.Title = "Signing Out...";
					loader.Show ();
					await Resolver.Resolve<IApiServices> ().PostAsync ("account/session/kill/" + App.MobileSession.Id, null);
					App.MobileSession.KillSession ();
					await Navigation.PopToRootAsync (true);
				}
			}

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

		}
	}
}


