using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Wubipaws.Mobile
{
	public partial class PetGroomingConfirmationPage : ContentPage
	{
		
		private DateTime minDate = new DateTime (1900, 1, 1);

		public PetGroomingConfirmationPage ()
		{
			InitializeComponent ();

		}

		protected PetGroomingConfirmationViewModel ViewModel
		{
			get { 
				return this.BindingContext as PetGroomingConfirmationViewModel;
			}

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			this.datePickerBordetella.Date = minDate;
			this.datePickerDHLPP.Date = minDate;
			this.datePickerRabies.Date = minDate;
		}
	}
}

