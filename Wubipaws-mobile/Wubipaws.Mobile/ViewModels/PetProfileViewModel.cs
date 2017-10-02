using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Ioc;
using Newtonsoft.Json.Linq;
using WubiPaws.DTO.JsonDocuments;
using System.Threading;
using WubiPaws.DTO;
using System.Threading.Tasks;

namespace Wubipaws.Mobile
{
	public class PetProfileViewModel : BaseViewModel
	{
		public PetProfileViewModel ()
		{
			this.SaveChangesCommand = new Command (SaveProfileChanges);
			this.ViewGroomingProfileCommand = new Command (ViewGroomingProfile);

			IsNew = true;
		}

		public string Id
		{
			get;
			set;
		}

		public string PetOwnerId
		{
			get;
			set;
		}

		public string PetOwnerName
		{
			get;
			set;
		}

		public bool IsNew
		{
			get;
			set;
		}

		public PetProfile PetDetails
		{
			get;
			set;
		}


		public ICommand SaveChangesCommand
		{
			get;
			protected set;
		}



		public ICommand ViewGroomingProfileCommand
		{
			get;
			protected set;
		}

	
		private async void ViewGroomingProfile ()
		{
			
			await Navigation.PushModalAsync<PetGroomingProfileViewModel> ((v, p) =>
			{
				v.Id = this.Id;
				v.PetOwnerId = this.PetOwnerId;
				v.PetGroomingDetails = new PetProfile () {

					GroomingProfileDocument = this.PetDetails.GroomingProfileDocument,
					ProfileDocument = this.PetDetails.ProfileDocument
				};
			}, true);
		}

	
		private async void SaveProfileChanges ()
		{
			//determine if new or existing
			var response = new ApiResponse ();
			using (var l = App.GetLoadingDialog (AppSettings.Constants.SaveLoaderText))
			{
				
				l.Show ();
				if (!IsNew)
				{
					response = await Api.PostAsync ("pet/" + this.Id, JObject.FromObject (new{
						petOwnerId = this.PetOwnerId,
						petOwnerName = this.PetOwnerName,
						profile = JObject.FromObject (this.PetDetails.ProfileDocument) 
					}));


				} else
				{
					response = await Api.PostAsync ("pet/account/" + App.MobileSession.Id, JObject.FromObject (new{
						petOwnerId = this.PetOwnerId,
						petOwnerName = this.PetOwnerName,
						profile = JObject.FromObject (this.PetDetails.ProfileDocument) 
					}));
				}
				l.Hide ();
			}

			if (response.IsSuccessStatusCode)
			{
				MessagingCenter.Send<PetProfileViewModel> (this, "data.reload");
				await DisplayAlert (AppSettings.Constants.SaveSuccessDlgTitle, AppSettings.Constants.SaveSuccessDlgMessage, "OK");
				await Navigation.PopAsync ();
			} else
			{
				await DisplayAlert (AppSettings.Constants.SaveErrorDlgTitle, AppSettings.Constants.SaveErrorDlgMessage, "OK");

			}
		}

	}

	public class PetProfile
	{
		public PetProfile ()
		{
			ProfileDocument = new Profile ();
			ProfileDocument.age = new Age ();
			GroomingProfileDocument = new GroomingProfile ();
		}


		public Profile ProfileDocument
		{
			get;
			set;
		}

		public GroomingProfile GroomingProfileDocument
		{
			get;
			set;
		}
	}
}

