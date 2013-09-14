using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Waveface.Client
{

	/// <summary>
	/// IntroLayout.xaml 的互動邏輯
	/// </summary>
	public partial class IntroLayout : UserControl
	{
		public event EventHandler CloseButtomClicked;


		public static readonly DependencyProperty NavigatorImageUriProperty = DependencyProperty.Register("NavigatorImageUriProperty", typeof(Uri), typeof(IntroLayout));
		public static readonly DependencyProperty AllowCloseProperty = DependencyProperty.Register("AllowCloseProperty", typeof(bool), typeof(IntroLayout), new PropertyMetadata(false));

		public Uri NavigatorImageUri
		{
			get { return (Uri)GetValue(NavigatorImageUriProperty); }
			set { SetValue(NavigatorImageUriProperty, value); }
		}

		public bool AllowClose
		{
			get { return (bool)GetValue(AllowCloseProperty); }
			set { SetValue(AllowCloseProperty, value); }
		}

		public IntroLayout()
		{
			InitializeComponent();
		}

		private void CloseImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var handler = CloseButtomClicked;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
