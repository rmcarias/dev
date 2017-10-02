using System;
using XLabs.Forms.Mvvm;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;
using WubiPaws.DTO.JsonDocuments;
using WubiPaws.DTO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Wubipaws.Mobile
{
	public class PetServicesViewModel : BaseViewModel
	{
		public PetServicesViewModel ()
		{
			

		}

		public string HeaderText
		{
			get { 
				return "Services Provided by " + CompanyInformation.GetDefault ().CompanyName;
			}
		}

		public List<DynamicJObjectViewModel> PetServices
		{
			get;
			set;
		}

		public async Task LoadContents ()
		{
			if (this.PetServices != null)
			{
				await Task.Yield ();
				return;
			}

			this.PetServices = new List<DynamicJObjectViewModel> ();
			var loadDefault = false;
			var response = new ApiResponse ();


			response =	await Api.GetAsync ("petservices");
			loadDefault = !response.IsSuccessStatusCode;

			if (!loadDefault)
			{
				var dto = new PetServiceListDocument ();

				loadDefault = !ObjectExtensions.TryDeserializeJson<PetServiceListDocument> (response.Data.ToString (), out dto);
				this.PetServices.Add (new DynamicJObjectViewModel (JObject.FromObject (new{
					Name = dto.servicesProvided [0].serviceName,
					Description = dto.servicesProvided [0].description,
					ImageFile = "scissors35.png"
				})));
				this.PetServices.Add (new DynamicJObjectViewModel (JObject.FromObject (new{
					Name = dto.servicesProvided [1].serviceName,
					Description = dto.servicesProvided [1].description,
					ImageFile = "boarding.png"
				})));
			}


			if (loadDefault)
			{
				this.PetServices.Add (new DynamicJObjectViewModel (JObject.FromObject (new{
					Name = "Grooming",
					Description = "Give your furry friend a fabulous spa day with our excellent grooming packages. Tap to reserve your spot.",
					ImageFile = "scissors35.png"
				})));
				this.PetServices.Add (new DynamicJObjectViewModel (JObject.FromObject (new{
					Name = "Boarding",
					Description = @"Going away for the weekend or taking a vacation? Tap to find out about our excellent overnight/weekend boarding services!",
					ImageFile = "boarding.png"
				})));
			}
		}
	}
}

