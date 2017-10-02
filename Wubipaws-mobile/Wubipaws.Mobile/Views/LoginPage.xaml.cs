using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Controls;

namespace Wubipaws.Mobile
{

	public partial class LoginPage : ContentPage
	{
	
		public LoginPage ()
		{
			InitializeComponent ();
			this.Padding = new Thickness (0, 0, 0, 0);
			NavigationPage.SetHasBackButton (this, false);

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			#if DEBUG
			this.TextEmail.Text = "devadmin@wubipaws.com";
			this.TextPassword.Text = "sugarplum";
			#endif
			this.TextEmail.Keyboard = Keyboard.Email;		
			this.ButtonCancel.IsVisible = !(this.BindingContext as LoginViewModel).IsDefaultPage;
		}

	}
}

