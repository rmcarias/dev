using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;

namespace Wubipaws.Mobile
{

	public enum GroomingScheduleViewOptions
	{
		Today,
		Tomorrow,
		Week
	}

	public class PetGroomingScheduleViewModel : BaseViewModel
	{
		public PetGroomingScheduleViewModel ()
		{
			this.PetServicesGroomingScheduleDictionary = new Dictionary<GroomingScheduleViewOptions,List<object>> ();
			this.PetServicesGroomingSlots = new List<object> ();

//			int todayDayOfWeek = (int)DateTime.Today.DayOfWeek;
//			if ((todayDayOfWeek >= 0 &&
//			    todayDayOfWeek <= 5))
//			{
//				
//				SeedGroomingSlots (10, 17);
//			} else if (todayDayOfWeek == (int)DayOfWeek.Saturday)
//			{
//				SeedGroomingSlots (9, 13);
//			}

			SelectedGroomingDate = DateTime.Now.ToString ("d");
		}

	

		public async Task LoadGroomingSlots ()
		{
			var response = await Api.GetAsync ("petservices/master/grooming/schedule/current");
			this.PetServicesGroomingScheduleDictionary = new Dictionary<GroomingScheduleViewOptions, List<object>> ();
			this.PetServicesGroomingSlots = new List<object> ();

			if (response.IsSuccessStatusCode)
			{
				if (response.HasData && response.Data != null)
				{
					var gs = response.Data.Value<JArray> ("groomingSchedule").AsJEnumerable ();

				
					Func<string,JArray> findSlotsForDay = (namedDay) =>
					{
						return	gs
							.Where (x => x.Value<string> ("namedDay") == namedDay)
							.Select (x => x.Value<JArray> ("slots"))
							.FirstOrDefault ();
					};			

					var today = FillSlotsArray (findSlotsForDay (DateTime.Today.DayOfWeek.ToString ()));
					var tomorrow = FillSlotsArray (findSlotsForDay (DateTime.Today.AddDays (1).DayOfWeek.ToString ()));

					#if DEBUG
					today = FillSlotsArray (findSlotsForDay (DayOfWeek.Tuesday.ToString ()));
					#endif
					this.PetServicesGroomingScheduleDictionary.Add (GroomingScheduleViewOptions.Today, today);
					this.PetServicesGroomingScheduleDictionary.Add (GroomingScheduleViewOptions.Tomorrow, tomorrow);
					this.PetServicesGroomingSlots = today;
				}

			}
		}

		private List<object> FillSlotsArray (JArray slots)
		{
			var l = new List<object> ();
			if (slots != null)
			{
				foreach (JToken k in slots)
				{
					var ts = TimeSpan.Parse (k.Value<string> ());
					l.Add (new{

						GroomingTimeSlotText = DateTime.Today.Add (ts).ToString ("h:mm tt"),
						GroomingTimeSlotHour = ts.Hours,
						IconText = ""
					});
				}
			}


			return l;
		}

		public GroomingScheduleViewOptions SelectedGroomingScheduleViewOption
		{
			get;
			set;
		}

		string groomingDate = string.Empty;

		public string SelectedGroomingDate
		{
			get { 
				return groomingDate;
			}

			set { 
				SetProperty (ref groomingDate, value);
			}
		}

		string selectedTimeSlot = string.Empty;

		public string SelectedTimeSlot
		{
			get { 
				return selectedTimeSlot;
			}

			set { 
				SetProperty (ref selectedTimeSlot, value);
			}
		}

		public  List<object> PetServicesGroomingSlots
		{
			get;
			set;
		}

		public  Dictionary<GroomingScheduleViewOptions, List<object>> PetServicesGroomingScheduleDictionary
		{
			get;
			set;
		}

		public async Task<bool> IsGroomingSlotAvailable ()
		{
			var postData = JObject.FromObject (new{

				date = SelectedGroomingDate,
				time = SelectedTimeSlot
			});

			var response = await Api.PostAsync ("petservices/check/available/groomingslot", postData);
			if (response.IsSuccessStatusCode)
			{
				var isSlotAvailable = response.Data.Value<bool> ("isSlotAvailable");
				return isSlotAvailable;
			}
			return false;
		}

		 
	}
}

