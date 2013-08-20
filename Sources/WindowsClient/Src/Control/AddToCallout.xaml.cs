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
	/// AddToCallout.xaml 的互動邏輯
	/// </summary>
	public partial class AddToCallout : UserControl
	{
		public string SelectionText
		{
			get { return (string)GetValue(SelectionTextProperty); }
			set { SetValue(SelectionTextProperty, value); }
		}

		public static readonly DependencyProperty SelectionTextProperty =
			DependencyProperty.Register("SelectionText", typeof(string), typeof(AddToCallout), null);

		public event EventHandler<AlbumClickedEventArgs> AlbumClicked;

		public AddToCallout()
		{
			InitializeComponent();
		}

		private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			var handler = AlbumClicked;
			if (handler != null)
			{
				var elem = e.OriginalSource as FrameworkElement;
				if (elem == null)
					return;

				handler(this, new AlbumClickedEventArgs { DataContext = elem.DataContext });
			}
		}
	}

	public class AlbumClickedEventArgs : EventArgs
	{
		public object DataContext { get; set; }

		public AlbumClickedEventArgs()
		{
		}
	}
}
