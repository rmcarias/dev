using System;
using Xamarin.Forms.Platform.iOS;
using Wubipaws.Mobile;
using Wubipaws.Mobile.iOS;
using System.ComponentModel;
using Xamarin.Forms;
using System.Diagnostics;
using UIKit;
using CoreAnimation;

[assembly: ExportRenderer (typeof(CircleImage), typeof(ImageCircleRenderer))]
[assembly: ExportRenderer (typeof (CustomExtendedEntry), typeof (CustomExtendedEntryRenderer))]
namespace Wubipaws.Mobile.iOS
{
	/// <summary>
	/// ImageCircle Implementation
	/// http://blog.xamarin.com/elegant-circle-images-in-xamarin.forms/
	/// </summary>
	public class ImageCircleRenderer : ImageRenderer
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init ()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged (e);
			if (Element == null)
				return;
			CreateCircle ();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			if (e.PropertyName == VisualElement.HeightProperty.PropertyName ||
			    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
			    e.PropertyName == CircleImage.BorderColorProperty.PropertyName ||
			    e.PropertyName == CircleImage.BorderThicknessProperty.PropertyName)
			{
				CreateCircle ();
			}
		}

		private void CreateCircle ()
		{
			try
			{
				double min = Math.Min (Element.Width, Element.Height);
				Control.Layer.CornerRadius = (float)(min / 2.0);
				Control.Layer.MasksToBounds = false;
				Control.Layer.BorderColor = ((CircleImage)Element).BorderColor.ToCGColor ();
				Control.Layer.BorderWidth = ((CircleImage)Element).BorderThickness;
				Control.ClipsToBounds = true;
			} catch (Exception ex)
			{
				Debug.WriteLine ("Unable to create circle image: " + ex);
			}
		}
	}

	public class CustomExtendedEntryRenderer : XLabs.Forms.Controls.ExtendedEntryRenderer 
	{ 

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);
			if (Control != null) {
				var extText = (CustomExtendedEntry)Element;
				if (extText?.HasBorder == true) {
					//to do border below
					//var borderLayer = new CALayer ();
					//borderLayer.MasksToBounds = true;
					//borderLayer.Frame = new CoreGraphics.CGRect (0f, extText.Height, extText.Width, 1f);
					//borderLayer.BorderColor = UIColor.Blue.CGColor;
					//borderLayer.BorderWidth = 1.0f;
					//borderLayer.CornerRadius = 1;
					//Control.Layer.AddSublayer (borderLayer);
					if (extText.HasRoundedBorder == false) {
						Control.Layer.BorderColor = extText.BorderColor.ToCGColor ();
						Control.Layer.BorderWidth = 1f;
						Control.BorderStyle = UITextBorderStyle.Line;
					}
				}
			}
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

		}
	}
}

