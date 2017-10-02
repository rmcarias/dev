using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using XLabs.Forms.Mvvm;
using System.Threading.Tasks;

namespace Wubipaws.Mobile
{

	public partial class MyProfilePage : ContentPage
	{

		public MyProfilePage ()
		{
			InitializeComponent ();
			this.Title = "My Profile";
			this.Icon = (FileImageSource)FileImageSource.FromFile ("ic_account_box_black_24dp.png");
			var tItem = new ToolbarItem () {
				Text = "Save",
				Order = ToolbarItemOrder.Primary,
				Priority = 2

			};

			tItem.Command = new Command (x =>
			{
				if (ViewModel.SaveChangesCommand != null)
				{

					ViewModel.SaveChangesCommand.Execute (null);
				}

			});
			this.ToolbarItems.Add (tItem);
		
		}

		protected AccountViewModel ViewModel
		{
			get { 
				return this.BindingContext as AccountViewModel;
			}
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			using (var l = App.GetLoadingDialog (AppSettings.Constants.LoaderText))
			{
				l.Show ();
				await ViewModel.LoadModel ();

				this.AccountSection.BindingContext = ViewModel.AccountDetails;
//				var genderOptions = ObjectExtensions.GenderOptions ();
//				this.GenderPicker.PopulatePicker (genderOptions);
//				this.GenderPicker.SelectedIndexChanged += (object sender, EventArgs e) =>
//				{
//					var picker = sender as Picker; 
//					if (picker == null || picker.SelectedIndex < 0)
//					{
//						return;
//					}
//					ViewModel.ProfileDetails ["gender"] = picker.Items [picker.SelectedIndex];
//				};
//				this.GenderPicker.SetValue (Picker.SelectedIndexProperty, genderOptions.FindIndex (x => x.Text == ViewModel.ProfileDetails ["gender"]));
				this.ProfileSection.BindingContext = ViewModel.ProfileDetails;

			}


		}
	}
}

