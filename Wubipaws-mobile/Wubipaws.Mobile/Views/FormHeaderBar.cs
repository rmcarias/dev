using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs.Forms.Mvvm;
using System.Threading.Tasks;
using XLabs.Forms.Controls;
using XLabs;

namespace Wubipaws.Mobile
{
	public class FormHeaderBar : ContentView
	{
		public FormHeaderBar ()
		{

			object headerStyle = new Style (typeof(Label));
			Application.Current.Resources.TryGetValue ("HeaderDefaultStyleLabel", out headerStyle);
			var label = new Label () {
				Style = (Style)headerStyle
			};

			StackLayout sl = new StackLayout ();
			sl.Orientation = StackOrientation.Horizontal;
			sl.HorizontalOptions = LayoutOptions.FillAndExpand;
			sl.Spacing = 0;

			label.SetBinding (Label.TextProperty, "Text");
			label.SetBinding (Label.TextColorProperty, "HeaderTextColor");
			label.SetBinding (Label.FontSizeProperty, "HeaderFontSize");
			label.HorizontalTextAlignment = TextAlignment.Center;

			label.BindingContext = this;

			var titleContentView = new ContentView () {
				Padding = new Thickness (0, 10, 0, 10),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = label
			};
	
			sl.Children.Add (titleContentView);

			var buttonContentView = new ContentView ();

			buttonContentView.SetBinding (ContentView.IsVisibleProperty, "HasCloseButton");
			buttonContentView.BindingContext = this;


			IconButton closeButton = new IconButton ();
			closeButton.Font = Font.SystemFontOfSize (17);
			closeButton.FontFamily = "fontawesome";
			closeButton.Icon = "";
			if (Device.OS == TargetPlatform.Android)
			{
				closeButton.Text = "";
				closeButton.IconFontName = "fa-close";
			}

			closeButton.IconSize = 17f;
			closeButton.BackgroundColor = Color.Transparent;
			closeButton.BorderColor = Color.Transparent;
			closeButton.IconColor = Color.FromHex ("#444444");
			closeButton.TextColor = Color.FromHex ("#444444");
			closeButton.WidthRequest = 24;
			closeButton.HeightRequest = 24;
			closeButton.HorizontalOptions = LayoutOptions.Center;

			closeButton.Clicked += async (object sender, EventArgs e) =>
			{
				await Task.Yield ();
				if (CloseButtonClicked != null)
				{
					CloseButtonClicked (sender, e);
				} else
				{
					//attempt to pop the stack
					try
					{
						await Navigation.PopModalAsync ();
					} catch
					{
					}

				}
			};
			buttonContentView.HorizontalOptions = LayoutOptions.End;
			buttonContentView.Content = closeButton;
			sl.Children.Add (buttonContentView);
			var cntView = new ContentView () {
				Padding = new Thickness (0, 0, 0, 0),
				Content = sl
			};
			cntView.SetBinding (ContentView.BackgroundColorProperty, "HeaderBackgroundColor");
			cntView.BindingContext = this;
			Frame f = new Frame ();
			f.HasShadow = true;

			f.Padding = new Thickness (0, 0, 0, 0);
			f.Content = cntView;
			this.Padding = new Thickness (0, 0, 0, 8);
			this.Content = f;

		}

		public static readonly BindableProperty HeaderTextProperty =
			BindableProperty.Create ("Text", typeof(string), typeof(FormHeaderBar), default(string), BindingMode.OneWay);

		public static readonly BindableProperty HeaderHasCloseButtonProperty =
			BindableProperty.Create ("HasCloseButton", typeof(bool), typeof(FormHeaderBar), false, BindingMode.OneWay);

		public static readonly BindableProperty HeaderBackgroundColorProperty =
			BindableProperty.Create ("HeaderBackgroundColor", typeof(Color), typeof(FormHeaderBar), Color.FromHex ("#E91E63"), BindingMode.OneWay);

		public static readonly BindableProperty HeaderTextColorProperty =
			BindableProperty.Create ("HeaderTextColor", typeof(Color), typeof(FormHeaderBar), Color.FromHex ("#FAFAFA"), BindingMode.OneWay);

		public static readonly BindableProperty HeaderFontSizeProperty =
			BindableProperty.Create ("HeaderFontSize", typeof(double), typeof(FormHeaderBar), Device.GetNamedSize (NamedSize.Large, typeof(Label)), BindingMode.OneWay);

		public string Text
		{
			get{ return (string)this.GetValue (HeaderTextProperty); }
			set {
				SetValue (HeaderTextProperty, string.IsNullOrEmpty (value) ? "" : value.ToUpper ());
			}
		}

		public bool HasCloseButton
		{
			get{ return (bool)this.GetValue (HeaderHasCloseButtonProperty); }
			set {
				SetValue (HeaderHasCloseButtonProperty, value);
			}
		}

		public Color HeaderBackgroundColor
		{
			get{ return (Color)this.GetValue (HeaderBackgroundColorProperty); }
			set {
				SetValue (HeaderBackgroundColorProperty, value);
			}
		}

		public Color HeaderTextColor
		{
			get{ return (Color)this.GetValue (HeaderTextColorProperty); }
			set {
				SetValue (HeaderTextColorProperty, value);
			}
		}

		public double HeaderFontSize
		{
			get{ return (double)this.GetValue (HeaderFontSizeProperty); }
			set {
				SetValue (HeaderFontSizeProperty, value);
			}
		}

		public event EventHandler CloseButtonClicked;

		protected override void OnChildAdded (Element child)
		{
			
			base.OnChildAdded (child);
		}
	}
}


