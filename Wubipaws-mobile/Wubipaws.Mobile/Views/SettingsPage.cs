using System;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Caching;

namespace Wubipaws.Mobile
{
	public class SettingsPage : BaseView
	{
		Switch switchOpt;

		public SettingsPage ()
		{

			this.Title = "App Settings";
			this.Icon = (FileImageSource)FileImageSource.FromFile ("ic_menu_black_24dp.png");
			CreateSettings ();

		}

		private void CreateSettings ()
		{
			ListView settingsView = new ListView ();
			if (Device.OS == TargetPlatform.iOS)
			{
				settingsView.RowHeight = 45;
				settingsView.HeightRequest = 90;

			} else
			{
				settingsView.RowHeight = 60;
				settingsView.HeightRequest = 120;
			}

			settingsView.VerticalOptions = LayoutOptions.Start;
			var collection = new List<SettingsListItemViewModel> ();
			collection.Add (new SettingsListItemViewModel () { 
				HeaderText = "About",
				ItemText = "Information about us", 

				OnItemClicked = async (itemSender) =>
				{
					try
					{
						var viewModel = new HybridWebViewModel ();
						viewModel.Title = "About";

						await base.Navigation.PushModalAsync (new HybridWebPageRenderer (viewModel, "home/about"));
					} catch (Exception)
					{
						await DisplayAlert ("Error", AppSettings.Constants.DisplayPageErrorMessage, "OK");
					}

				}
			});
			collection.Add (new SettingsListItemViewModel () { 
				HeaderText = "Help", 
				ItemText = "Application Help",
				OnItemClicked = async (itemSender) =>
				{
				
					try
					{
						var viewModel = new HybridWebViewModel ();
						viewModel.Title = "Help";

						await base.Navigation.PushModalAsync (new HybridWebPageRenderer (viewModel, "home/help"));
					} catch (Exception)
					{
						await DisplayAlert ("Error", AppSettings.Constants.DisplayPageErrorMessage, "OK");
					}
				}
			});



			settingsView.ItemsSource = collection;
			settingsView.ItemTemplate = new DataTemplate (typeof(TextCell));		
			settingsView.ItemTemplate.SetBinding (TextCell.TextProperty, "HeaderText");
			settingsView.ItemTemplate.SetValue (TextCell.TextColorProperty, Color.FromHex ("#E91E63"));
			settingsView.ItemTemplate.SetBinding (TextCell.DetailProperty, "ItemText");
			settingsView.ItemTemplate.SetValue (TextCell.DetailColorProperty, Color.FromHex ("#37474F"));

			settingsView.ItemSelected += async (object sender, SelectedItemChangedEventArgs e) =>
			{
				await Task.Yield ();
				var itemObject = e.SelectedItem  as SettingsListItemViewModel;
				if (itemObject != null)
				{
					if (itemObject.OnItemClicked != null)
					{
						itemObject.OnItemClicked (sender);
					}

				}
				settingsView.SelectedItem = null;

			};

			var sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.Padding = new Thickness (12, 8, 5, 5);
			var lbl = new Label ();
			lbl.Text = "Set Sign In page as your home page?";
			lbl.FontAttributes = FontAttributes.Bold;
			lbl.FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label));
			lbl.HorizontalOptions = LayoutOptions.FillAndExpand;
			lbl.VerticalOptions = LayoutOptions.Center;
			switchOpt = new Switch ();
			switchOpt.SetValue (Switch.IsToggledProperty, GetSwitchLandingPageOptionValue ());
			switchOpt.Toggled += async (object sender, ToggledEventArgs e) =>
			{
				await Task.Yield ();
				try
				{
					var choice = e.Value;
					if (choice)
					{	
						App.Current.Properties [AppSettings.Constants.AppLaunchSettingsCacheKey] = DefaultLandingScreen.SignIn;
					} else
					{
						App.Current.Properties [AppSettings.Constants.AppLaunchSettingsCacheKey] = DefaultLandingScreen.Home;
					}
					await App.Current.SavePropertiesAsync ();
				} catch (Exception)
				{
					await DisplayAlert ("Error", AppSettings.Constants.DisplayPageErrorMessage, "OK");
				}
			};
			sl.Children.Add (lbl);
			sl.Children.Add (switchOpt);
			this.Content = new StackLayout () {

				Children = { 
					settingsView,
					sl
				}

			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		
		}

		private bool GetSwitchLandingPageOptionValue ()
		{

			if (App.Current.Properties.ContainsKey (AppSettings.Constants.AppLaunchSettingsCacheKey))
			{
				var enu = (DefaultLandingScreen)App.Current.Properties [AppSettings.Constants.AppLaunchSettingsCacheKey];
				if (enu == DefaultLandingScreen.SignIn)
					return true;
			}
			return false;
		}
	}
}


