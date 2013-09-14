using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Waveface.Client
{
	public partial class AddToCallout : UserControl
	{
		public String SelectionText
		{
			get { return (String)GetValue(SelectionTextProperty); }
			set { SetValue(SelectionTextProperty, value); }
		}

		public static readonly DependencyProperty SelectionTextProperty =
			DependencyProperty.Register("SelectionText", typeof(String), typeof(AddToCallout), null);

		public event EventHandler<AlbumClickedEventArgs> AlbumClicked;

		public AddToCallout()
		{
			InitializeComponent();
		}

		private void Border_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
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
		public Object DataContext { get; set; }

		public AlbumClickedEventArgs()
		{
		}
	}
}
