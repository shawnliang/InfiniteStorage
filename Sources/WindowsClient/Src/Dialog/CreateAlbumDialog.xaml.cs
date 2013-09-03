using System;
#region

using System.Windows;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class CreateAlbumDialog : Window
	{
		public String CreateName
		{
			get { return tbxAlbumName.Text; }
			set { tbxAlbumName.Text = value; }
		}

		public String DefaultName
		{
			get { return tbxAlbumName.Text; }
			set { tbxAlbumName.Text = value; }
		}

		public CreateAlbumDialog()
		{
			InitializeComponent();
		}

		private void Button_Click(Object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Button_Click_1(Object sender, RoutedEventArgs e)
		{
			OK();
		}

		private void OK()
		{
			if (String.IsNullOrWhiteSpace(tbxAlbumName.Text))
			{
				tbxAlbumName.Focus();
				return;
			}

			DialogResult = true;
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			tbxAlbumName.SelectAll();
			tbxAlbumName.Focus();
		}

		private void tbxFavoriteName_KeyDown(Object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			OK();
		}
	}
}