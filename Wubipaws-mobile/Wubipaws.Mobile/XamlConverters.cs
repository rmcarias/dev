using System;
using Xamarin.Forms;

namespace Wubipaws.Mobile
{
	#region Converters
	public class InvertBoolenConverter : IValueConverter
	{

		#region IValueConverter implementation

		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			if (value is bool)
			{

				return !(bool)value;
			}
			return value;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}

	public class PascalCaseStringConverter : IValueConverter
	{

		#region IValueConverter implementation

		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			if ((value is string) && value != null)
			{

				return value.ToString ().SplitPascalCase ();
			}
			return value;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((value is string) && value != null)
			{

				return value.ToString ().Replace (" ", "");
			}
			return value;
		}

		#endregion
	}
	#endregion
}

