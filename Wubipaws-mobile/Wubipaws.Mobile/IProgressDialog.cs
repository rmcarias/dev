using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Forms.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using XLabs.Ioc;

namespace Wubipaws.Mobile
{
	public interface IProgressDialog : IDisposable
	{

		string Title
		{
			get;
			set;
		}

		int PercentComplete
		{
			get;
			set;
		}

		bool IsDeterministic
		{
			get;
			set;
		}

		bool IsShowing
		{
			get;
		}

		void SetCancel (Action onCancel, string cancelText = "Cancel");

		void Show (string titleText="");

		void ShowToast (string text, bool isError = false);

		void Hide ();
	}
	
}
