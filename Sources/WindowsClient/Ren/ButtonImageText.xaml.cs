#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace Waveface.Client
{
	public partial class ButtonImageText : UserControl
	{
		#region Text Property

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set
			{
				SetValue(TextProperty, value);

				if (string.IsNullOrEmpty(value))
					tbText.Visibility = Visibility.Collapsed;
			}
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ButtonImageText), null);

		#endregion

		#region ImageUri Property

		public Uri ImageUri
		{
			get { return (Uri)GetValue(ImageUriProperty); }
			set { SetValue(ImageUriProperty, value); }
		}

		public static readonly DependencyProperty ImageUriProperty =
			DependencyProperty.Register("ImageUri", typeof(Uri), typeof(ButtonImageText), null);

		#endregion

		#region ImageEnterUri Property

		public Uri ImageEnterUri
		{
			get { return (Uri)GetValue(ImageEnterUriProperty); }
			set { SetValue(ImageEnterUriProperty, value); }
		}

		public static readonly DependencyProperty ImageEnterUriProperty =
			DependencyProperty.Register("ImageEnterUri", typeof(Uri), typeof(ButtonImageText), null);

		#endregion

		#region ImageDisableUri Property

		public Uri ImageDisableUri
		{
			get { return (Uri)GetValue(ImageDisableUriProperty); }
			set { SetValue(ImageDisableUriProperty, value); }
		}

		public static readonly DependencyProperty ImageDisableUriProperty =
			DependencyProperty.Register("ImageDisableUri", typeof(Uri), typeof(ButtonImageText), null);

		#endregion

		#region ImagePressedUri Property

		public Uri ImagePressedUri
		{
			get { return (Uri)GetValue(ImagePressedUriProperty); }
			set { SetValue(ImagePressedUriProperty, value); }
		}

		public static readonly DependencyProperty ImagePressedUriProperty =
			DependencyProperty.Register("ImagePressedUri", typeof(Uri), typeof(ButtonImageText), null);

		#endregion

		#region DiableForeground Property

		public Brush DisableForeground
		{
			get { return (Brush)GetValue(DisableForegroundProperty); }
			set { SetValue(DisableForegroundProperty, value); }
		}

		public static readonly DependencyProperty DisableForegroundProperty =
			DependencyProperty.Register("DisableForeground", typeof(Brush), typeof(ButtonImageText), null);

		#endregion

		public ButtonImageText()
		{
			InitializeComponent();

			DataContext = this;
		}

		#region Click Event Procedure
		public delegate void ClickEventHandler(object sender, RoutedEventArgs e);

		public event ClickEventHandler Click;

		protected void RaiseClick(RoutedEventArgs e)
		{
			var handler = Click;
			if (handler != null)
				Click(this, e);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			RaiseClick(e);
		}
		#endregion
	}
}