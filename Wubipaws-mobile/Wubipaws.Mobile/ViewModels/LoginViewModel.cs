using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using XLabs.Forms.Services;
using Newtonsoft.Json;
using WubiPaws.DTO;


namespace Wubipaws.Mobile
{
	[ViewType (typeof(LoginPage))]
	public class LoginViewModel : BaseViewModel
	{

		public LoginViewModel ()
		{
			initCommands ();
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

		[JsonIgnore]
		public bool IsDefaultPage
		{
			get;
			set;
		}

		public ICommand LogInCommand
		{
			get;
			private set;
		}

		public ICommand CancelCommand
		{
			get;
			private set;
		}

		private void initCommands ()
		{

			CancelCommand = new Command (async () => await base.Navigation.PopAsync (true));
			//await base.Navigation.PopModalAsync (true));

			LogInCommand = new Command (async _ =>
			{
								
				if (this.UserEmail.IsNullEmptyOrWhiteSpace () || this.UserPassword.IsNullEmptyOrWhiteSpace ())
				{
					await DisplayAlert ("Sign In Error", "Your email and password must be provided!", "OK");
				} else
				{
					await base.SetIsBusy (true);
					ApiResponse response = new ApiResponse ();

					using (var l = App.GetLoadingDialog ("Signing In..."))
					{
						l.Show ();
						response = await Api.PostAsync ("account/auth", 
							new {
							userEmail = this.UserEmail,
							userPassword = this.UserPassword
						});
					}
					 
					await base.SetIsBusy (false);
					if (!response.IsSuccessStatusCode)
					{
						await DisplayAlert ("Sign In Error", "There was an error signing you in. Please try again.", "OK");

					} else
					{

						App.MobileSession = new MobileAuthSession (
							response.Data.Value<string> ("access_token"),
							response.Data.Value<string> ("expires_utc"),
							response.Data.Value<string> ("user_name"),
							this.UserEmail,
							this.UserPassword,
							response.Data.Value<string> ("id"));
						App.MobileSession.IsAuthorized = true;
							
						//	await base.Navigation.PopModalAsync (true);

						await Navigation.PushAsync (new AccountIndexPage (), true);
						//NavigationService.NavigateTo<AccountIndexPage> (animated: true);
					}

				}

				
			});

		}
	}
}

