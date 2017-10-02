using System;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Wubipaws.Mobile.Droid;
using XLabs.Forms.Controls;
using Xamarin.Forms;

[assembly: Xamarin.Forms.ExportRenderer (typeof(IconButton), typeof(Wubipaws.Mobile.Droid.IconButtonRenderer))]
namespace Wubipaws.Mobile.Droid
{
	public class IconButtonRenderer: ButtonRenderer
	{

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged (e);
			if (e.OldElement == null)
			{
				var ic = (Android.Widget.Button)Control; // for example
				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/fontawesome-webfont.ttf");
				Control.Typeface = font;
				ic.Typeface = font;
				ic.SetIncludeFontPadding (false);
				ic.SetPadding (3, 3, 3, 3);
			}
		}
	}
}

