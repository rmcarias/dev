using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ServiceModel.Channels;
using System.Windows.Input;
using System.Diagnostics;
using WubiPaws.DTO.JsonDocuments;

namespace Wubipaws.Mobile
{
	public partial class PetProfilePage : ContentPage
	{
		public PetProfilePage ()
		{
			InitializeComponent ();

		}

		protected PetProfileViewModel Pet
		{
			get { 
				return this.BindingContext as PetProfileViewModel;
			}
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

		}


	}
}

