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
		public static readonly DependencyProperty NavigatorImageUriProperty = DependencyProperty.Register("NavigatorImageUriProperty", typeof(Uri), typeof(IntroLayout));

		public Uri NavigatorImageUri
		{
			get { return (Uri)GetValue(NavigatorImageUriProperty); }
			set { SetValue(NavigatorImageUriProperty, value); }
		}

		public IntroLayout()
		{
			InitializeComponent();
		}
	}
}
