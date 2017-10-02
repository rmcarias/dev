using System;
using Xamarin.Forms;

namespace Wubipaws.Mobile
{
	public class CustomExtendedEntry : XLabs.Forms.Controls.ExtendedEntry
	{
		public CustomExtendedEntry () 
		{
		}

		/// <summary>
		/// The HasRoundedBorder property
		/// </summary>
		public static readonly BindableProperty HasRoundedBorderProperty =
			BindableProperty.Create ("HasRoundedBorder", typeof (bool), typeof (CustomExtendedEntry), true);

		/// <summary>
		/// Gets or sets if the border should be default rounded (ios) or flat (android style)
		/// </summary>
		public bool HasRoundedBorder {
			get { return (bool)GetValue (HasRoundedBorderProperty); }
			set { SetValue (HasRoundedBorderProperty, value); }
		}

		/// <summary>
		/// The BorderColor property
		/// </summary>
		public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create ("BorderColor", typeof (Color), typeof (CustomExtendedEntry), Color.Default);

		/// <summary>
		/// Gets or sets if the border color
		/// </summary>
		public Color BorderColor {
			get { return (Color)GetValue (BorderColorProperty); }
			set { SetValue (BorderColorProperty, value); }
		}
	}
}

