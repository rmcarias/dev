using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using WubiPaws.DTO;
using System.Collections.Generic;
using WubiPaws.DTO.JsonDocuments;

namespace Wubipaws.Mobile
{
	public class PetGroomingProfileViewModel : BaseViewModel
	{
		public PetGroomingProfileViewModel ()
		{
			this.SaveGroomingProfileChangesCommand = new Command (SaveGroomingProfileChanges);
			this.CancelGroomingProfileChangesCommand = new Command (CancelGroomingProfileChanges);
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


		public PetProfile PetGroomingDetails
		{
			get;
			set;

		}

		public ICommand SaveGroomingProfileChangesCommand
		{
			get;
			protected set;
		}

		public ICommand CancelGroomingProfileChangesCommand
		{
			get;
			protected set;
		}

		private async void CancelGroomingProfileChanges ()
		{
			await Navigation.PopAsync (true);
		}

		public List<OtherConditions> LoadOtherConditions ()
		{
			return GroomingProfile.GetOtherConditionsList ();
		}

		private async void SaveGroomingProfileChanges ()
		{
			var errs = new List<string> ();
			//Validate 
			if (FormIsValid (out errs))
			{
				var response = new ApiResponse ();
				var dataToSend = JObject.FromObject (new{
					petOwnerId = this.PetOwnerId,
					petOwnerName = App.MobileSession.AccountName,
					profile = JObject.FromObject (this.PetGroomingDetails.ProfileDocument),
					groomingProfile = JObject.FromObject (this.PetGroomingDetails.GroomingProfileDocument)
				});

				using (var l = App.GetLoadingDialog (AppSettings.Constants.SaveLoaderText))
				{

					l.Show ();
					response = await Api.PostAsync ("pet/" + this.Id, dataToSend);
				}
				if (!response.IsSuccessStatusCode)
				{
					await DisplayAlert (AppSettings.Constants.SaveErrorDlgTitle, AppSettings.Constants.SaveErrorDlgMessage);
				} else
				{
					MessagingCenter.Send<PetGroomingProfileViewModel> (this, "data.reload");
					await DisplayAlert (AppSettings.Constants.SaveSuccessDlgTitle, AppSettings.Constants.SaveSuccessDlgMessage);
					await Navigation.PopModalAsync ();

				}
			} else
			{
				errs.Insert (0, AppSettings.Constants.FormErrorDlgMessage + System.Environment.NewLine);
				var message = string.Join (System.Environment.NewLine, errs);
				await DisplayAlert (AppSettings.Constants.FormErrorDlgTitle, message);
			}

		}

		private bool FormIsValid (out List<string> errors)
		{ 

			var isValid = true;
			errors = new List<string> ();
			if (!this.PetGroomingDetails.GroomingProfileDocument.lastGroomingDate.IsNullEmptyOrWhiteSpace () &&
			    !this.PetGroomingDetails.GroomingProfileDocument.lastGroomingDate.IsDateTime ())
			{
				isValid = false;
				errors.Add ("Last date of grooming is not valid.");
			}

			if (!this.PetGroomingDetails.GroomingProfileDocument.notes.IsNullEmptyOrWhiteSpace () &&
			    this.PetGroomingDetails.GroomingProfileDocument.notes.Length > 255)
			{
				isValid = false;
				errors.Add ("Additional notes cannot exceed 255 characters");
			}

			return isValid;
		}

	}
}

