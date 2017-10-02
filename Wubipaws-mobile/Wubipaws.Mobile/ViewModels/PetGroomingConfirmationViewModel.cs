using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WubiPaws.DTO.JsonDocuments;
using System.Collections.Generic;
using WubiPaws.DTO;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Newtonsoft.Json.Schema;

namespace Wubipaws.Mobile
{
	public class PetGroomingConfirmationViewModel : BaseViewModel
	{
		string groomingSuccessMessage = "Congratulations!\rThank you {0} for trusting your pet to us.\rYour grooming appointment has been created for {1}.\rPlease contact the service provider Fido & Co. directly at 619-555-5555 if you have any further questions and/or concerns with this this appointment.\rOr if you need to cancel or make changes.\rThank you!";
		DateTime initDateTime = new DateTime (1900, 1, 1);
		DateTime minDateTime = new DateTime (2005, 1, 1);

		public PetGroomingConfirmationViewModel ()
		{
			ScheduleGroomingCommand = new Command (ScheduleGrooming);
			Rabies = DHLPP = Bordetella = minDateTime;
		}


		public string PetName
		{
			get;
			set;
		}

		public string GroomingDateTime
		{
			get;
			set;
		}

		public string GroomingDate
		{
			get;
			set;
		}

		public string GroomingTime
		{
			get;
			set;
		}


		public string GroomingConfScheduleText
		{
			get { 
				return string.Format ("Schedule grooming for {0} on {1}", PetName, GroomingDateTime);
			}
		}

		public string DeclarationText
		{
			get { 
				return	string.Format ("I hereby declare that my pet \"{0}\" is currently up to date on his/her vaccinations and he/she will be kept current for all future appointments.", PetName);
			}
		}

		public string WaiverText
		{
			get { 
				return	string.Format ("I waive and relinquish any and all claims against {0}," +
				" its employees and representatives, except those arising from the negligence on the part of the {0}" +
				" during this appointment and any future appointments.", 
					CompanyInformation.GetDefault ().CompanyName);
			}
		}

		public bool WaiverAccepted
		{
			get;
			set;
		}


		public bool	 VaccinationsCurrent
		{
			get;
			set;
		}

		private DateTime bordetella;

		public DateTime Bordetella
		{
			get { return bordetella; }
			set {
				SetProperty (ref bordetella, value, "Bordetella"); 
			}
		}

		private DateTime rabies;

		public DateTime	 Rabies
		{
			get{ return rabies; }

			set {
				SetProperty (ref rabies, value, "Rabies");
			}
		}

		private DateTime dhlpp;

		public DateTime	 DHLPP
		{
			get{ return dhlpp; }
			set { 
				SetProperty (ref dhlpp, value, "DHLPP");
			}
		}

		public string Notes
		{
			get;
			set;
		}

		public string OwnerContactEmail
		{
			get { 
				return App.MobileSession.LoginEmail;
			}
		}

		public PetProfileViewModel SelectedPetForGrooming
		{
			get;
			set;
		}

		public ICommand ScheduleGroomingCommand
		{
			get;
			protected set;
		}

		private async void ScheduleGrooming ()
		{
			
			var loader = App.GetLoadingDialog (AppSettings.Constants.SchedulingGroomingLoaderText);
			var apiResponse = new ApiResponse ();
			string scheduledAppointmentId = string.Empty;

			Action<string,bool> removeLoader = async  (txt, isError) =>
			{
				loader.Hide ();
				if (isError)
				{
					await DisplayAlert (AppSettings.Constants.DisplayGeneralErrorDlgTitle, txt, "OK");
				} else
				{
					await DisplayAlert ("Grooming Appointment Success", txt, "OK");
					await Navigation.PopModalAsync ();
				}

			};

			//Validate

			var errs = new List<string> ();
			if (!IsValidForm (errs))
			{
				errs.Insert (0, AppSettings.Constants.FormErrorDlgMessage + System.Environment.NewLine);
				var message = string.Join (System.Environment.NewLine, errs);
				await DisplayAlert (AppSettings.Constants.FormErrorDlgTitle, message);
				return;
			}

			loader.Show ();
			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.waiverAccepted = WaiverAccepted;
			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.vaccines.vaccinationsCurrent = VaccinationsCurrent;
			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.waiverAcceptedDate = DateTime.Now;

			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.vaccines.bordetella = Bordetella.ToString ("d");
			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.vaccines.rabies = Rabies.ToString ("d");
			SelectedPetForGrooming.PetDetails.GroomingProfileDocument.vaccines.dhlpp = DHLPP.ToString ("d");


			Debug.WriteLine (JsonConvert.SerializeObject (SelectedPetForGrooming.PetDetails, Newtonsoft.Json.Formatting.Indented));
			//Save Grooming First Because it may return a conflict
			apiResponse = await InvokeSaveGroomingAppointment ();

			if (!apiResponse.IsSuccessStatusCode)
			{
				if (apiResponse.StatusCode == (int)System.Net.HttpStatusCode.Conflict)
				{
					removeLoader ("We apologize, but the grooming appointment time is no longer available. Please select another.", true);
					return;
				}

				removeLoader ("We apologize, but we are unable to schedule your grooming appointment. Please try again.", true);
				return;
			}
			scheduledAppointmentId = apiResponse.Data.Value<String> ("confirmationId");
			#if DEBUG
			Debug.WriteLine ("scheduledAppointmentId:{0}", scheduledAppointmentId);
			#endif
			apiResponse = new ApiResponse ();
			apiResponse = await InvokeSaveProfileChanges ();
			//if it fails, then remove the appointment by id
		
			removeLoader (string.Format (groomingSuccessMessage, 
				SelectedPetForGrooming.PetOwnerName, GroomingDateTime), false);
			
			//Save to Cache
			await AppHelperFunctions.SaveGroomingAppointmentToCache (GroomingDate, GroomingTime);
		}

		private async Task<ApiResponse> InvokeSaveGroomingAppointment ()
		{
			
			var groomingApptDoc = new PetServiceScheduleDocument ();
			var response = new ApiResponse ();
			try
			{
				
				groomingApptDoc.companyRenderingService = CompanyInformation.GetDefault ().CompanyName;
				groomingApptDoc.petOwnerName = SelectedPetForGrooming.PetOwnerName;
				groomingApptDoc.createdAt = DateTime.Now;
				groomingApptDoc.updatedAt = DateTime.Now;
				groomingApptDoc.petId = SelectedPetForGrooming.Id;
				groomingApptDoc.petOwnerId = SelectedPetForGrooming.PetOwnerId;
				groomingApptDoc.servicesRendered = new PetServiceRendered ();
				groomingApptDoc.servicesRendered.groomings = new List<PetGroomingItem> ();
				groomingApptDoc.petName = SelectedPetForGrooming.PetDetails.ProfileDocument.name;
				groomingApptDoc.ownerContactEmail = OwnerContactEmail;
				var groomingItem = new PetGroomingItem () {
					agreementInitials = App.MobileSession.UserName,
					cancelled = false,
					priorGrooming = true,
					notes = Notes,
					charge = 0.00M,
					groomingScheduleItem = new GroomingScheduleItem () {
						date = DateTime.Parse (GroomingDate, CultureInfo.CurrentCulture),
						time = DateTime.ParseExact (GroomingTime, "h:mm tt", CultureInfo.CurrentCulture).TimeOfDay
					}
				};

				groomingApptDoc.servicesRendered.groomings.Add (groomingItem);
			} catch (Exception ex)
			{
				#if DEBUG
				Debug.WriteLine (ex.Message);
				#endif
				response.IsSuccessStatusCode = false;
				return response;
			}

			response = await Api.PostAsync ("petservices/schedule/grooming", groomingApptDoc);
			return response;
		}

		private async Task<ApiResponse> InvokeSaveProfileChanges ()
		{
			var profileDocumentObject = JObject.FromObject (SelectedPetForGrooming.PetDetails.ProfileDocument);
			var groomingProfileDocumentObject = JObject.FromObject (SelectedPetForGrooming.PetDetails.GroomingProfileDocument);
			var response = new ApiResponse ();
			try
			{
				response = await Api.PostAsync ("pet/" + SelectedPetForGrooming.Id, JObject.FromObject (new{
				petOwnerId = SelectedPetForGrooming.PetOwnerId,
				petOwnerName = SelectedPetForGrooming.PetOwnerName,
				profile = profileDocumentObject,
				groomingProfile = groomingProfileDocumentObject
				}));
			} catch (Exception ex)
			{
				#if DEBUG
				Debug.WriteLine (ex.Message);
				#endif
				response.IsSuccessStatusCode = false;
				return response;
			}

			return response;
		}

		private bool IsValidForm (List<string>  errors)
		{
			
			errors = errors ?? new List<string> ();
			bool formIsValid = true;
			if (!WaiverAccepted)
			{
				errors.Add ("You cannot proceed without accepting the waiver.");
				formIsValid = false;
			}

			if (!VaccinationsCurrent)
			{
				errors.Add ("Grooming services cannot be provided unless your pet is current on their vaccines.");
				formIsValid = false;
			}

			if ((Bordetella.Date > DateTime.Today ||
			    Bordetella.Date < minDateTime) ||
			    Bordetella.Date == initDateTime)
			{
				errors.Add ("Invalid date for Bordetella vaccination.");
				formIsValid = false;

			}

			if ((DHLPP.Date > DateTime.Today ||
			    DHLPP.Date < minDateTime) ||
			    DHLPP.Date == initDateTime)
			{
				errors.Add ("Invalid date for DHLPP vaccination.");
				formIsValid = false;
			}

			if ((Rabies.Date > DateTime.Today ||
			    Rabies.Date < minDateTime) ||
			    Rabies.Date == initDateTime)
			{
				errors.Add ("Invalid date for Rabies vaccination.");
				formIsValid = false;
			}


			return formIsValid;
		}
	}
}

