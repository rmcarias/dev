using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace Wubipaws.Mobile
{
	[ContentProperty ("Source")]
	public class ImageResourceExtension : IMarkupExtension
	{
		public ImageResourceExtension ()
		{

		}

		public string Source
		{
			get;
			set;
		}

		public object ProvideValue (IServiceProvider serviceProvider)
		{
			if (Source == null)
				return null;

			// Do your translation lookup here, using whatever method you require
			var imageSource = FileImageSource.FromFile (Source);

			return imageSource;
		}
	}
}

