#region

using System.Windows;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class CreateAlbumDialog : Window
	{
		public string CreateName
		{
			get { return tbxAlbumName.Text; }
			set { tbxAlbumName.Text = value; }
		}

		public string DefaultName
		{
			get { return tbxAlbumName.Text; }
			set { tbxAlbumName.Text = value; }
		}

		public CreateAlbumDialog()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			OK();
		}

		private void OK()
		{
			if (string.IsNullOrWhiteSpace(tbxAlbumName.Text))
			{
				tbxAlbumName.Focus();
				return;
			}

			DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tbxAlbumName.SelectAll();
			tbxAlbumName.Focus();
		}

		private void tbxFavoriteName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			OK();
		}
	}
}