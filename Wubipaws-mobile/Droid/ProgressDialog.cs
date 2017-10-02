using System;
using AndroidHUD;
using XHUD;
using Android.Graphics;
using Android.Widget;

namespace Wubipaws.Mobile.Droid
{
	public class ProgressDialog : IProgressDialog
	{

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


		private Action cancelAction;
		private string cancelText;

		public virtual void SetCancel (Action onCancel, string cancel)
		{
			this.cancelAction = onCancel;
			this.cancelText = cancel;
		}


		public virtual void Show (string titleText = "")
		{
			if (this.IsShowing)
				return;
			
			this.title = titleText;
			this.IsShowing = true;
			this.Refresh ();
		}

		public void ShowToast (string text, bool isError = false)
		{
			this.IsShowing = true;
			var view = AndHUD.Shared.CurrentDialog.FindViewById<TextView> (AndroidHUD.Resource.Id.textViewStatus);
			if (isError)
			{

				view.SetTextColor (Color.Rgb (244, 67, 54));
			} else
			{


				view.SetTextColor (Color.Rgb (0, 178, 16));
			}
			Utils.RequestMainThread (() =>
			{
				AndHUD.Shared.ShowToast (
					Utils.GetActivityContext (),
					text, 
					AndroidHUD.MaskType.Clear, 
					TimeSpan.FromMilliseconds (2000), true);
			});

		}

		public virtual void Hide ()
		{
			this.IsShowing = false;
			Utils.RequestMainThread (() => AndHUD.Shared.Dismiss (Utils.GetActivityContext ()));
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

			var p = -1;
			var txt = this.Title;
			if (this.IsDeterministic)
			{
				p = this.PercentComplete;
				if (!String.IsNullOrWhiteSpace (txt))
					txt += "\n";

				txt += p + "%\n";
			}

			if (this.cancelAction != null)
				txt += "\n" + this.cancelText;

			Utils.RequestMainThread (() => AndHUD.Shared.Show (
				Utils.GetActivityContext (), 
				txt,
				p, 
				AndroidHUD.MaskType.Black,
				null,
				this.OnCancelClick
			));
		}


		private void OnCancelClick ()
		{
			if (this.cancelAction == null)
				return;

			this.Hide ();
			this.cancelAction ();
		}

		#endregion


	}
}

