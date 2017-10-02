using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Ioc;
using System.Threading.Tasks;
using XLabs.Forms.Mvvm;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using XLabs.Forms.Services;
using Newtonsoft.Json;
using WubiPaws.DTO.JsonDocuments;
using XLabs.Serialization;
using System.Dynamic;

namespace Wubipaws.Mobile
{
	public partial class PetsList : ContentPage
	{
		public PetsList ()
		{
			InitializeComponent ();
			this.Title = "My Pets";
			var addItem = new ToolbarItem () {
				Text = "+Add",
				Order = ToolbarItemOrder.Primary,
				Priority = 2

			};
		
			addItem.Clicked += OnAddToolbarItemClicked;
			this.ToolbarItems.Add (addItem);

		}


		protected PetListingViewModel ViewModel
		{
			get{ return (this.BindingContext as PetListingViewModel); }
			set { this.BindingContext = value; }
		}

		protected override async void OnAppearing ()
		{
			
			this.ModalHeader.IsVisible = ViewModel.ShowHeader;
			this.ModalHeader.Text = this.Title;
			await RunInit ();
			base.OnAppearing ();

		}

		protected async void OnAddToolbarItemClicked (object sender, EventArgs e)
		{
			NavigationPage.SetBackButtonTitle (this, "My Pets");
			await ShowProfileScreen (null, true);	
		}

		protected async void OnItemSelected (object sender, ItemTappedEventArgs args)
		{
			var item = args.Item as PetListItem;
			if (item == null)
			{
				this.PetList.SelectedItem = null;
				return;
			}
			bool extractedOk = false;
			var profileVm = ExtractExistingPetProfileInformation (item, out extractedOk);
			if (ViewModel.OnPetListItemSelected != null)
			{
				if (!ViewModel.OnPetListItemSelected (profileVm))
				{
					return;
				}
			}	
			await ShowProfileScreen (profileVm, extractedOk);		
			// Reset the selected item
			this.PetList.SelectedItem = null;
		}


		private async Task ShowProfileScreen (PetProfileViewModel item, bool dataExtractedOk)
		{
			if (dataExtractedOk)
			{
				var page = ViewFactory.CreatePage<PetProfileViewModel,PetProfilePage> ((v, p) =>
				{
					if (item != null)
					{

						v.IsNew = false;
						v.PetOwnerName = item.PetOwnerName;
						v.PetOwnerId = App.MobileSession.Id;
						v.Id = item.Id;
						v.PetDetails = item.PetDetails;


					} else
					{
						v.IsNew = true;
						v.PetDetails = new PetProfile () {
							ProfileDocument = new Profile (),
							GroomingProfileDocument = new GroomingProfile ()
						};
						v.Id = "";
						v.PetOwnerName = App.MobileSession.AccountName;
						v.PetOwnerId = App.MobileSession.Id;

					}

				}) as PetProfilePage;
				await Navigation.PushAsync (page, true);
			} else
			{
				await DisplayAlert (AppSettings.Constants.DisplayGeneralErrorDlgTitle, AppSettings.Constants.DisplayGeneralErrorDlgMessage, "OK");
			}


		}

		private PetProfileViewModel ExtractExistingPetProfileInformation (PetListItem item, out bool extractedOk)
		{
			extractedOk = false;
			var pv = new PetProfileViewModel ();
			pv.PetDetails = new PetProfile () {
				ProfileDocument = new Profile (),
				GroomingProfileDocument = new GroomingProfile ()
			};

			extractedOk = ObjectExtensions.TryPopuateObject (item.profileJsonString, pv.PetDetails.ProfileDocument);
			if (extractedOk)
			{
				pv.Id = item.id;
				pv.PetOwnerName = item.petOwnerName != null
				&& item.petOwnerName.Equals (App.MobileSession.AccountName) ? item.petOwnerName : 
									App.MobileSession.AccountName;
				pv.PetOwnerId = item.petOwnerId;
				if (!item.groomingProfileJsonString.IsNullEmptyOrWhiteSpace ())
				{
					pv.PetDetails.GroomingProfileDocument.otherConditions.Clear ();
					ObjectExtensions.TryPopuateObject (item.groomingProfileJsonString, pv.PetDetails.GroomingProfileDocument);

				}
				pv.IsNew = false;
			}

			return pv;
		}

		private async Task RunInit ()
		{
			var l = App.GetLoadingDialog (AppSettings.Constants.LoaderText);
			try
			{
				l.Show ();

				await ViewModel.LoadPets ();
				this.PetList.ItemsSource = ViewModel.MyPets;
				Application.Current.Properties [AppSettings.Constants.AppPetProfileCacheKey] = ViewModel.MyPets.Count;	
				l.Hide ();

				
			} catch (Exception)
			{
				l.Hide ();
				await DisplayAlert ("Error", "There was an error loading the page.", "OK");	
			}
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			this.PetList.ItemsSource = null;
		}
	}
}

