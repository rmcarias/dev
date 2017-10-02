using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using Newtonsoft.Json.Linq;

namespace Wubipaws.Mobile
{
	public partial class PetServicesListPage : ContentPage
	{
		private PetServicesViewModel viewModel;
		private CompanyInformation company;

		public PetServicesListPage ()
		{
			InitializeComponent ();
			this.Icon = (FileImageSource)FileImageSource.FromFile ("ic_list_black_48dp.png");
			this.Title = "Pet Services";

		}

		protected async override void OnAppearing ()
		{
			
			base.OnAppearing ();

			this.viewModel = this.BindingContext as PetServicesViewModel;
			IProgressDialog l = App.GetLoadingDialog (AppSettings.Constants.LoaderText);
			l.Show ();
			try
			{
				await viewModel.LoadContents ();
				l.Hide ();
			} catch (Exception)
			{
				l.Hide ();
				await DisplayAlert (AppSettings.Constants.DisplayGeneralErrorDlgTitle, AppSettings.Constants.DisplayGeneralErrorDlgMessage, "OK");	
			}

			company = CompanyInformation.GetDefault ();
			this.PetServicesList.ItemsSource = viewModel.PetServices;

		}


		public async void OnItemSelected (object sender, ItemTappedEventArgs args)
		{
			var item = args.Item as DynamicJObjectViewModel;
			if (item == null)
				return;


			if (item.Value<string> ("name") == "Boarding")
			{
				
				string message = AppHelperFunctions
								.CreateServiceContactInformationMessage ("boarding", company.CompanyName, company.Email, company.PhoneNumber);
				await DisplayAlert ("Boarding Coming Soon!", message, "OK");	
			} else
			{
				await Navigation.PushAsync (new PetGroomingScheduleList ());
			}
			PetServicesList.SelectedItem = null;
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			this.PetServicesList.ItemsSource = null;
		}
	}
}

