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
	/// ShareCallout.xaml 的互動邏輯
	/// </summary>
	public partial class ShareCallout : UserControl
	{
		public string SelectionText
		{
			get { return (string)GetValue(SelectionTextProperty); }
			set { SetValue(SelectionTextProperty, value); }
		}

		public static readonly DependencyProperty SelectionTextProperty =
			DependencyProperty.Register("SelectionText", typeof(string), typeof(ShareCallout), null);


		public event EventHandler CreateOnlineAlbumClicked;
		public event EventHandler SaveToClicked;

		public ShareCallout()
		{
			InitializeComponent();
		}

		private void btnOnlineAlbum_click(object sender, RoutedEventArgs e)
		{
			var handler = CreateOnlineAlbumClicked;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private void btnSaveTo_click(object sender, RoutedEventArgs e)
		{
			var handler = SaveToClicked;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
