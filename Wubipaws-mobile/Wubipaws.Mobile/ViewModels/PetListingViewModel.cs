using System;
using XLabs.Forms.Mvvm;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using XLabs.Ioc;
using XLabs.Caching;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Wubipaws.Mobile
{
	public class PetListingViewModel : BaseViewModel
	{
		string CACHE_KEY = "__PETLIST__:{0}";

		public PetListingViewModel ()
		{
			
			CACHE_KEY = string.Format (CACHE_KEY, App.MobileSession.Id);
			MessagingCenter.Subscribe<PetGroomingProfileViewModel> (this, "data.reload", (sender) =>
			{
				ResetPetsList ();	
			});
			MessagingCenter.Subscribe<PetProfileViewModel> (this, "data.reload", (sender) =>
			{
				ResetPetsList ();
			});

		}

		public object HeaderText
		{
			get { 
				return "Your Pets";
			}
		}

		public List<PetListItem> MyPets
		{
			get;
			set;
		}

		public Func<PetProfileViewModel,bool> OnPetListItemSelected
		{
			get;
			set;
		}

		public bool ShowHeader
		{
			get;
			set;
		}

		public async Task LoadPets (bool reload = false)
		{

			await Task.Yield ();
			if (MyPets == null || reload)
			{
				var response = await Api.GetAsync ("pet/account/" + App.MobileSession.Id);
				if (response.IsSuccessStatusCode && response.Data != null)
				{
					
					MyPets = new List<PetListItem> ();
					var petListing = response.Data.SelectToken ("pets").AsJEnumerable ();
					foreach (JToken item in petListing)
					{
						MyPets.Add (new PetListItem {
							petOwnerId = item.Value<string> ("petOwnerId"),
							id = item.Value<string> ("id"),
							petOwnerName = item.Value<string> ("petOwnerName"),
							name = item ["profile"].Value<string> ("name"),
							description = string.Format ("{0} is a {1}, {2}", 
								item ["profile"].Value<string> ("gender").ToUpper () == "FEMALE" ? "She" : "He",
								item ["profile"].Value<string> ("gender").ToLower (),
								item ["profile"].Value<string> ("breed").ToUpper ()),
							profileJsonString = item ["profile"].ToString (Newtonsoft.Json.Formatting.None),
							groomingProfileJsonString = item.Value<JObject> ("groomingProfile") != null ? 
								item ["groomingProfile"].ToString (Newtonsoft.Json.Formatting.None) :
								null

						});

					}

					//cache.Add (CACHE_KEY, MyPets);
				}
			}
		}

		private void ResetPetsList ()
		{
			MyPets = null;
		}
	}
}

