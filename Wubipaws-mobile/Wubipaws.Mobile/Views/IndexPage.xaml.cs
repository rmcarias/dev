using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using XLabs.Ioc;
using System.Diagnostics;
using XLabs.Caching;
using XLabs.Forms.Controls;
using XLabs;
using System.Linq.Expressions;


namespace Wubipaws.Mobile
{


	public partial class IndexPage : ContentPage
	{
	
		public IndexPage ()
		{
			InitializeComponent ();
			this.ButtonShowLogin.Command = new Command (async (sender) =>
			{
				var page = ViewFactory.CreatePage<LoginViewModel,LoginPage> () as LoginPage;
				//await Navigation.PushModalAsync (page);
				await Navigation.PushAsync (page);
			});

			this.ButtonShowRegister.Command = new Command (async () =>
			{
				var page = ViewFactory.CreatePage<RegisterViewModel,RegisterPage> () as RegisterPage;
				//await Navigation.PushModalAsync (page);
				await Navigation.PushAsync (page);
			});

	
		}


		protected override  void OnAppearing ()
		{
			
			base.OnAppearing ();
		
		}
	}
}

