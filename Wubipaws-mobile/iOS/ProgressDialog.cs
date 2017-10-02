using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Security.Policy;
using System.IO;
using Xamarin.Forms.Platform;
using XLabs.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Platform.Mvvm;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using System.Net;
using XLabs.Serialization;
using Newtonsoft.Json;
using BigTed;
using Xamarin.Forms;

namespace Wubipaws.Mobile.iOS
{
	public class ProgressDialog : IProgressDialog
	{
		public ProgressDialog ()
		{

		}

		#region IProgressDialog Members

		private string title;

		public virtual string Title
		{
			get { return this.title; }
			set {  
				if (this.title == value)
					return;

				this.title = value;
				this.Refresh ();
			}
		}

		private int percentComplete;

		public virtual int PercentComplete
		{
			get { return this.percentComplete; }
			set {
				if (this.percentComplete == value)
					return;

				if (value > 100)
				{
					this.percentComplete = 100;
				} else if (value < 0)
				{
					this.percentComplete = 0;
				} else
				{
					this.percentComplete = value;
				}

				this.Refresh ();
			}
		}

		public virtual bool IsDeterministic
		{
			get;
			set;
		}

		public virtual bool IsShowing
		{
			get;
			private set;
		}

		private string cancelText;
		private Action cancelAction;

		public virtual void SetCancel (Action onCancel, string cancel)
		{
			this.cancelAction = onCancel;
			this.cancelText = cancel;
			this.Refresh ();
		}

		public virtual void Show (string titleText = "")
		{
			if (this.IsShowing)
				return;
			
			this.title = titleText;
			this.IsShowing = true;
			this.Refresh ();
		}

		public virtual void ShowToast (string text, bool isError = false)
		{
			this.IsShowing = true;
			if (isError)
			{
				//ProgressHUD.Shared.HudBackgroundColour = UIColor.FromRGB (255, 205, 210);
				ProgressHUD.Shared.HudForegroundColor = UIColor.FromRGB (244, 67, 54);
			} else
			{
				//ProgressHUD.Shared.HudBackgroundColour = UIColor.FromRGB (200, 230, 201);
				ProgressHUD.Shared.HudForegroundColor = UIColor.FromRGB (0, 178, 16);
			}
		
			BTProgressHUD.ShowToast (text, ProgressHUD.MaskType.Clear, false, 2000);
		}

		public virtual void Hide ()
		{
			this.IsShowing = false;
			BTProgressHUD.Dismiss ();
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose ()
		{
			this.Hide ();
		}

		#endregion

		#region Internals

		protected virtual void Refresh ()
		{
			if (!this.IsShowing)
				return;
			ProgressHUD.Shared.HudForegroundColor = UIColor.FromRGB (245, 0, 87);//#F50057 (pink)
			var txt = this.Title;
			float p = -1;
			if (this.IsDeterministic)
			{
				p = (float)this.PercentComplete / 100;
				if (!String.IsNullOrWhiteSpace (txt))
				{
					txt += "... ";
				}
				txt += this.PercentComplete + "%";
			}

			if (this.cancelAction == null)
			{

			
				BTProgressHUD.Show (
					this.Title,
					p,
					ProgressHUD.MaskType.Gradient
				);
			} else
			{
				BTProgressHUD.Show (
					this.cancelText, 
					this.cancelAction,
					txt,
					p,
					ProgressHUD.MaskType.Gradient
				);
			}
		}

		#endregion
	}
	
}
