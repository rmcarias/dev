using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Serialization;
using System.Threading.Tasks;
using System.Dynamic;
using XLabs.Forms.Mvvm;

namespace Wubipaws.Mobile
{
	public partial class PetGroomingScheduleList : ContentPage
	{
		SegmentControl segment = new SegmentControl ();
		PetGroomingScheduleViewModel viewModel;
		static string noGroomingAvailForDayErrorMessage = "Grooming appointments are only available Tuesday-Saturday.";
		dynamic selectedGroomingListItem;

		public PetGroomingScheduleList ()
		{
			InitializeComponent ();
			viewModel = new PetGroomingScheduleViewModel ();
			this.BindingContext = viewModel;
			Activate ();
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			
			if (!AppHelperFunctions.UserHasRegisteredPets ())
			{
				this.PetGroomingServicesList.IsVisible = false;

				this.UnauthorizedText.IsVisible = true;
				await DisplayAlert ("Feature Disabled", 
					@"In order to use this feature, you must first create your pet's basic and grooming profile from the 'My Pets' tab.", "OK");
				
				return;
			}
			this.GroomingSelectionViewContainer.IsVisible =
			this.PetGroomingServicesList.IsVisible = true;
				
			using (var l = App.GetLoadingDialog (AppSettings.Constants.LoaderText))
			{
				l.Show ();
				try
				{
					await viewModel.LoadGroomingSlots ();
					this.PetGroomingServicesList.ItemsSource = null;
					this.PetGroomingServicesList.ItemsSource = viewModel.PetServicesGroomingSlots;

					l.Hide ();
					if (viewModel.PetServicesGroomingSlots.Count == 0)
					{
						await DisplayAlert ("Load Error", "There are no grooming appointments available for today.", "OK");
					}

				} catch
				{
					l.Hide ();
					await DisplayAlert (AppSettings.Constants.DisplayGeneralErrorDlgTitle, AppSettings.Constants.DisplayGeneralErrorDlgMessage, "OK");
				}

			}
		}

	

		private void Activate ()
		{

			segment.AddSegment ("Today");
			segment.AddSegment ("Tomorrow");
			segment.AddSegment ("Week");
			segment.TintColor = AppSettings.Constants.DarkGrayBackgroundTintColor;
			segment.SelectedSegmentChanged += SelectedSegmentItemChanged;

			(segment.Content as StackLayout).Children [2].IsEnabled = false; //The "Week" option is disabled for this release
			if (Device.OS == TargetPlatform.Android)
			{
				((segment.Content as StackLayout).Children [2] as Button).TextColor = Color.FromHex ("#cccccc");
				((segment.Content as StackLayout).Children [2] as Button).Opacity = .50;
			}

			this.GroomingSelectionViewContainer.Content = new StackLayout () {
				Children = { segment },
				IsVisible = AppHelperFunctions.UserHasRegisteredPets ()
			};

		}

		public async void OnItemSelected (object sender, ItemTappedEventArgs args)
		{
			var item = args.Item.CastTo (new{

				GroomingTimeSlotText = "",
				GroomingTimeSlotHour = 0,
				IconText = ""
			});

			if (item == null)
				return;
			
			if (AppHelperFunctions.UserHasAlreadyBookedGroomingAppointmentFor 
				(viewModel.SelectedGroomingDate, item.GroomingTimeSlotText))
			{
			
				var response =	await DisplayAlert ("Schedule Conflict", "You have already scheduled an appointment for today. Proceeding will override your current schedule", "OK", "Cancel");
				if (!response)
					return;
			}

			var errMessage = string.Empty;
			if (!IsSchedulingAvailForViewOption (item, out errMessage))
			{
				await DisplayAlert ("Scheduling Error", errMessage, "OK");
				return;
			}

			if (!await IsSlotAvailableOnServer (item))
			{
				await DisplayAlert ("Scheduling Error", "The selected grooming time slot is already booked. Please select another.", "OK");
				return;
			}



			var page = (PetsList)ViewFactory.CreatePage<PetListingViewModel,PetsList> ((v, p) =>
			{
				v.OnPetListItemSelected = this.OnPetSelected;
				v.ShowHeader = true;
			});
			selectedGroomingListItem = item;
			await Navigation.PushModalAsync (page);
		}

		private bool OnPetSelected (PetProfileViewModel selectedPet)
		{
			Navigation.PopModalAsync (true);
			var page = ViewFactory.CreatePage<PetGroomingConfirmationViewModel,PetGroomingConfirmationPage> ((v, p) =>
			{

				v.PetName = selectedPet.PetDetails.ProfileDocument.name;
				v.GroomingDateTime = viewModel.SelectedGroomingDate + " @ " + selectedGroomingListItem.GroomingTimeSlotText;
				v.SelectedPetForGrooming = selectedPet;
				v.GroomingDate = viewModel.SelectedGroomingDate;
				v.GroomingTime = selectedGroomingListItem.GroomingTimeSlotText;

			});
			Navigation.PushModalAsync (page as PetGroomingConfirmationPage);
			return false;
		}

		private async void SelectedSegmentItemChanged (object sender, int segmentIndex)
		{
			await Task.Yield ();
			segment.TintColor = AppSettings.Constants.DarkGrayBackgroundTintColor;
			viewModel.SelectedGroomingDate = DateTime.Now.ToString ("d");
			switch (segmentIndex)
			{

			case 0:
				viewModel.SelectedGroomingScheduleViewOption = GroomingScheduleViewOptions.Today;
				break;
			case 1:
				viewModel.SelectedGroomingScheduleViewOption = GroomingScheduleViewOptions.Tomorrow;
				if (!IsSchedulingAvailForNextDay ())
				{
					await DisplayAlert ("Scheduling Unavailable", noGroomingAvailForDayErrorMessage, "OK");
					viewModel.SelectedGroomingScheduleViewOption = GroomingScheduleViewOptions.Today;
					segment.SelectedSegment = 0;
				}
				viewModel.SelectedGroomingDate = DateTime.Now.AddDays (1).ToString ("d");
				break;
			case 2:
				segment.TintColor = Color.Gray;
				break;
			}
		}

		private async Task<bool> IsSlotAvailableOnServer (dynamic selectedListItem)
		{
			viewModel.SelectedTimeSlot = selectedListItem.GroomingTimeSlotText;
			var isAvailable = await viewModel.IsGroomingSlotAvailable ();
			return isAvailable;
		}

		private bool IsSchedulingAvailForViewOption (dynamic selectedListItem, out string error)
		{
			error = noGroomingAvailForDayErrorMessage;
			if (viewModel.SelectedGroomingScheduleViewOption == GroomingScheduleViewOptions.Today)
			{
				if (DateTime.Today.DayOfWeek == DayOfWeek.Monday ||
				    DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
				{
					#if !DEBUG
					return false;
					#endif
				}

				#if !DEBUG
				if (DateTime.Now.Hour > selectedListItem.GroomingTimeSlotHour)
				{
					error = "Grooming appointment is no longer available for your selected time: " + selectedListItem.GroomingTimeSlotText + ".";
					return false;
				}
				#endif
			} else
			{
				if (!IsSchedulingAvailForNextDay ())
				{
					return false;
				}
			}

		
			error = "";
			return true;
		}

		private bool IsSchedulingAvailForNextDay ()
		{

			if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday ||
			    DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
			{
				return false;
			}
			return true;
		}
	}
}

