#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

		public ButtonImageText()
		{
			InitializeComponent();

			DataContext = this;
		}

		#region Click Event Procedure

		private void borMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			RaiseClick(e);
		}

		public delegate void ClickEventHandler(object sender, RoutedEventArgs e);

		public event ClickEventHandler Click;

		protected void RaiseClick(RoutedEventArgs e)
		{
			if (null != Click)
				Click(this, e);
		}

		#endregion

		private void borMain_MouseEnter(object sender, MouseEventArgs e)
		{
			VisualStateManager.GoToElementState(borMain, "MouseEnter", true);
		}

		private void borMain_MouseLeave(object sender, MouseEventArgs e)
		{
			VisualStateManager.GoToElementState(borMain, "MouseLeave", true);
		}
	}
}