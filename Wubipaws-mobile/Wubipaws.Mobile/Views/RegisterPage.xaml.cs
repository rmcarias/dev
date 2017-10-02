using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Linq.Expressions;
using XLabs.Forms.Validation;
using System.Text.RegularExpressions;

namespace Wubipaws.Mobile
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage ()
		{

			InitializeComponent ();
			NavigationPage.SetHasBackButton (this, false);
		}


		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			this.TextEmail.Keyboard = Keyboard.Email;

		}
	}
}

