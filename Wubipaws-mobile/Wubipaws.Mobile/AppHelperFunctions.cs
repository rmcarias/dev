using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Wubipaws.Mobile
{
	public static class AppHelperFunctions
	{

		public static string CreateServiceContactInformationMessage (string serviceName, string companyName, string email, string phone)
		{
			return $"Please contact {companyName} for details about {serviceName} your pet." +
				   $"{System.Environment.NewLine}Email: {email}{System.Environment.NewLine}Phone: {phone}"; 
				//companyName, serviceName, email, phone, System.Environment.NewLine);
		}

		public static bool UserHasRegisteredPets ()
		{
			object count = null;
			Application.Current.Properties.TryGetValue (AppSettings.Constants.AppPetProfileCacheKey, out count);
			if (count == null)
				return false;

			return (int)count > 0;
		}

		public static async Task SaveGroomingAppointmentToCache (string date, string time)
		{
			var currentList = GetGroomingAppointmentsFromCache ();
			string dateAndTime = string.Format ("{0}:{1}", date, time);
				
			if (currentList.Contains (dateAndTime))
			{
				currentList.Remove (dateAndTime);
			}
			currentList.Add (dateAndTime);
			Application.Current.Properties [AppSettings.Constants.GroomingAppointmentsCacheKey] = JsonConvert.SerializeObject (currentList);
			await Application.Current.SavePropertiesAsync ();
		}

		public static bool UserHasAlreadyBookedGroomingAppointmentFor (string date, string time)
		{
			string dateAndTime = string.Format ("{0}:{1}", date, time);
			var groomingAppointmentsList = GetGroomingAppointmentsFromCache ();
			if (groomingAppointmentsList.Count > 0)
			{
				return groomingAppointmentsList.Any (x => x.Equals (dateAndTime, StringComparison.OrdinalIgnoreCase));
			}
			return false;

		}

		private static List<string> GetGroomingAppointmentsFromCache ()
		{
			object appointmentsCollection = null;
			Application.Current.Properties.TryGetValue (AppSettings.Constants.GroomingAppointmentsCacheKey, out appointmentsCollection);
			if (appointmentsCollection != null)
				return JsonConvert.DeserializeObject<List<string>> (appointmentsCollection.ToString ());
			return new List<string> ();
		}
	}
}

