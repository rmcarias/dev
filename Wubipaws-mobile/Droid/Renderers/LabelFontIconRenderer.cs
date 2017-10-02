using System;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using Wubipaws.Mobile;
using Wubipaws.Mobile.Droid;

[assembly: Xamarin.Forms.ExportRenderer (typeof(LabelFontIcon), typeof(LabelFontIconRenderer))]
namespace Wubipaws.Mobile.Droid
{
	public class LabelFontIconRenderer : LabelRenderer
	{
		public LabelFontIconRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			base.OnElementChanged (e);
			if (e.OldElement == null)
			{
				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/fontawesome-webfont.ttf");
				Control.Typeface = font;
			}
		}
	}
}

