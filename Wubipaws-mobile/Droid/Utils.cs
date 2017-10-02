using System;
using System.Threading;
using Xamarin.Forms;
using Android.Content;

namespace Wubipaws.Mobile.Droid
{

	public static class Utils
	{

		public static void RequestMainThread (Action action)
		{
			if (Android.App.Application.SynchronizationContext == SynchronizationContext.Current)
				action ();
			else
				Android.App.Application.SynchronizationContext.Post (x => MaskException (action), null);
		}


		public static void MaskException (Action action)
		{
			try
			{
				action ();
			} catch
			{
			}
		}


		public static Context GetActivityContext ()
		{
			return Forms.Context;
		}
	}
}

