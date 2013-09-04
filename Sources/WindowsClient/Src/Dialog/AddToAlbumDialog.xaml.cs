using System;
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

		public Object SelectedItem
		{
			get { return cbxAlbumName.SelectedItem; }
			set { cbxAlbumName.SelectedItem = value; }
		}

		public Int32 SelectedIndex
		{
			get { return cbxAlbumName.SelectedIndex; }
			set { cbxAlbumName.SelectedIndex = value; }
		}

		public AddToAlbumDialog()
		{
			InitializeComponent();
		}

		private void Button_Click(Object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Button_Click_1(Object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			cbxAlbumName.Focus();
		}
	}
}