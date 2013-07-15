#region

using System.Collections;
using System.Windows;

#endregion

namespace Waveface.Client
{
	public partial class AddToAlbumDialog : Window
	{
		public IEnumerable ItemSource
		{
			get { return cbxAlbumName.ItemsSource; }
			set
			{
				cbxAlbumName.ItemsSource = value;

				if (cbxAlbumName.HasItems)
					cbxAlbumName.SelectedIndex = 0;
			}
		}

		public object SelectedItem
		{
			get { return cbxAlbumName.SelectedItem; }
			set { cbxAlbumName.SelectedItem = value; }
		}

		public int SelectedIndex
		{
			get { return cbxAlbumName.SelectedIndex; }
			set { cbxAlbumName.SelectedIndex = value; }
		}

		public AddToAlbumDialog()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cbxAlbumName.Focus();
		}
	}
}