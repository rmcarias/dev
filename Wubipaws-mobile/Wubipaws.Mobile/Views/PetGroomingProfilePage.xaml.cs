using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using WubiPaws.DTO.JsonDocuments;
using XLabs.Forms.Controls;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Wubipaws.Mobile
{
	public partial class PetGroomingProfilePage : ContentPage
	{
		public PetGroomingProfilePage ()
		{
			InitializeComponent ();

		}

		protected PetGroomingProfileViewModel ViewModel
		{
			get { 
				return (this.BindingContext as PetGroomingProfileViewModel);
			}
		}


		private void Init ()
		{


			if (ViewModel.PetGroomingDetails.GroomingProfileDocument.otherConditions.Count == 0)
			{
				ViewModel.PetGroomingDetails.GroomingProfileDocument.otherConditions = ViewModel.LoadOtherConditions ();
			}

			this.OtherConditionsList.ItemsSource = new ObservableCollection<OtherConditions> (ViewModel.PetGroomingDetails.GroomingProfileDocument.otherConditions);
			this.ModalHeader.CloseButtonClicked += CloseModal;
		}


		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			Init ();
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			this.OtherConditionsList.ItemsSource = null;
			this.ModalHeader.CloseButtonClicked -= CloseModal;
		}

		private async void CloseModal (object sender, EventArgs e)
		{
			await Navigation.PopModalAsync (true);
		}


	}
}

