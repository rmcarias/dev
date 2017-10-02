using System;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using WubiPaws.DTO;
using System.Threading.Tasks;

namespace Wubipaws.Mobile
{
	public class RegisterViewModel : BaseViewModel
	{
		public RegisterViewModel ()
		{
			CancelCommand = new Command (async () =>
			{
				//await Navigation.PopModalAsync (true);
				await Navigation.PopAsync (true);
			});

			RegisterCommand = new Command (RegisterAccountCommand);
		}

		private string userEmail;

		public string UserEmail
		{
			get{ return this.userEmail; }
			set{ this.SetProperty (ref this.userEmail, value); }
		}

		private string userPassword;

		public string UserPassword
		{
			get{ return this.userPassword; }
			set{ this.SetProperty (ref this.userPassword, value); }
		}

		private string name;

		public string Name
		{
			get{ return this.name; }
			set{ this.SetProperty (ref this.name, value); }
		}

		private bool confirmPassword;

		public bool ConfirmPassword
		{
			get{ return this.confirmPassword; }
			set { 
				this.SetProperty (ref this.confirmPassword, value);

			}
		}

		public ICommand RegisterCommand
		{
			get;
			protected set;
		}

		public ICommand CancelCommand
		{
			get;
			protected set;
		}

		private async void RegisterAccountCommand ()
		{
		
			if (this.name.IsNullEmptyOrWhiteSpace () ||
			    this.userEmail.IsNullEmptyOrWhiteSpace () ||
			    this.userPassword.IsNullEmptyOrWhiteSpace ())
			{
				await DisplayAlert ("Registration Error", "There are errors with the form. All fields are required.", "OK");
				return;
			}

			if (!this.userEmail.IsValidEmail ())
			{
				await DisplayAlert ("Registration Error", "There are errors with the form. Email format was not recognized.", "OK");
				return;
			}



			var response = new ApiResponse ();
			using (var l = App.GetLoadingDialog ("Registering..."))
			{
				l.Show ();
				//check if account exists
				response = await Api.GetAsync ("account/check/exists/?email=" + System.Net.WebUtility.UrlEncode (this.userEmail));
				if (response.IsSuccessStatusCode)
				{
					var exists = response.Data.Value<string> ("message");
					if (exists == "yes")
					{
						l.Hide ();
						await DisplayAlert ("Registration Error", "The email you entered is already registered to another user. Please use a different email or sign in.", "OK");
						return;
					}
				} else
				{
					l.Hide ();
					await DisplayAlert ("Registration Error", "There was an error creating your account. Please try again.", "OK");
					return;
				}

				//Continue With Login
				response = await Api.PostAsync ("account/register", new
				{
					accountName = this.Name,
					userEmail = this.UserEmail,
					userName = this.UserEmail,
					userPassword = this.UserPassword
				});

			}

			if (!response.IsSuccessStatusCode)
			{
				await DisplayAlert ("Registration Error", "There was an error creating your account. Please try again.", "OK");

			} else
			{
				await DisplayAlert ("Registration", "Account created. You will now be redirected to sign in.");
				//await Navigation.PopModalAsync (false);
				//await Navigation.PushModalAsync<LoginViewModel> (true);
				await Navigation.PushAsync<LoginViewModel> (true);
			}



		}
	}
}

