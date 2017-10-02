using System;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Wubipaws.Mobile
{
	public class BaseViewModel : ViewModel
	{
		public BaseViewModel ()
		{
			
		}

		[JsonIgnore]
		private bool isBusy = false;

		[JsonIgnore]
		public new bool IsBusy
		{

			get{ return this.isBusy; }
			set{ this.SetProperty (ref this.isBusy, value); }
		}

		[JsonIgnore]
		public IApiServices Api
		{ 
			get { 
				return Resolver.Resolve<IApiServices> ();
			}
		}

		public async Task SetIsBusy (bool val)
		{
			await Task.Yield ();
			IsBusy = !val;
		
		}

		public async Task DisplayAlert (string title, string message, string acceptText = "OK", string cancelText = null, Action<bool> callback = null)
		{

			if (Application.Current.MainPage != null)
			{
				bool diagResponse = false;
				if (!cancelText.IsNullEmptyOrWhiteSpace ())
				{
					diagResponse = await Application.Current.MainPage.DisplayAlert (title, message, acceptText, cancelText);
				} else
				{
					await Application.Current.MainPage.DisplayAlert (title, message, acceptText);
				}

				if (callback != null)
				{
					callback (diagResponse);
				}
			}

		}
	}
}

