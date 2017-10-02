using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;
using WubiPaws.DTO;

namespace Wubipaws.Mobile
{
	public class AccountViewModel : BaseViewModel
	{
		public AccountViewModel ()
		{
			SaveChangesCommand = new Command( async () => await SaveChanges());
		}

		#region Account Details

		public DynamicJObjectViewModel AccountDetails
		{
			get;
			set;
		}

		#endregion

		#region Profile Details

		public DynamicJObjectViewModel ProfileDetails
		{
			get;
			set;
		}

		#endregion

		public ICommand	 SaveChangesCommand
		{
			get;
			private set;
		}

		public async Task LoadModel ()
		{
			var response = new ApiResponse ();
			if (ProfileDetails == null)
			{
				ProfileDetails = new DynamicJObjectViewModel (new JObject ());
				response = await Api.GetAsync ("account/" + App.MobileSession.Id);
				if (response.IsSuccessStatusCode)
				{
					JToken token = response.Data.SelectToken ("profile", false);
					JObject profileData = token == null || !token.HasValues ? JObject.FromObject (new{
						firstName = "",
						lastName = "",
						address = "",
						city = "",
						state = "",
						postalCode = "",
						phoneNumber = ""

					}) : (JObject)token;
					ProfileDetails = new DynamicJObjectViewModel (profileData);
					App.MobileSession.AccountName = response.Data.Value<string> ("accountName");
					this.AccountDetails = new DynamicJObjectViewModel (
						JObject.FromObject (new
						{
							accountName = App.MobileSession.AccountName,
							email = App.MobileSession.LoginEmail,
							password = App.MobileSession.LoginPassword
						}));
				} else
				{
					if (response.IsUnAuthorizedError)
					{
						await DisplayAlert ("Session Expired", "Your session has expired. Please sign in again.", "OK");
						App.MobileSession.KillSession ();
					}
				}
			}
				
		}

		private async Task SaveChanges ()
		{
			await base.SetIsBusy (true);
			var errMessage = "There was an error saving your changes. Please try again.";
			var err = false;
			try
			{
				using (var loading = Resolver.Resolve<IProgressDialog> ())
				{
					loading.Title = "Saving...";
					loading.Show ();
					var response = await Api.PostAsync ("account/" + App.MobileSession.Id, JObject.FromObject (new{
						userEmail = this.AccountDetails ["email"],
						userPassword = this.AccountDetails ["password"],
						profile = JObject.Parse (this.ProfileDetails.ToString ())
					}));
					loading.Hide ();
					if (response.IsSuccessStatusCode)
					{

						await DisplayAlert ("Saved", "Changes Saved");

					} else
					{
						await DisplayAlert ("Error", errMessage);
					}

				}
				
			} catch (Exception)
			{
				err = true;
			} finally
			{

				await base.SetIsBusy (false);
			}


			if (err)
			{
				await DisplayAlert ("Error", errMessage);
			}


		}
			
	}
}

