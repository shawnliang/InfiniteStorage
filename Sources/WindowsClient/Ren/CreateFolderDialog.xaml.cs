#region

using System.Windows;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class CreateFolderDialog : Window
	{
		public string FolderName
		{
			get { return tbxFolderName.Text; }
			set { tbxFolderName.Text = value; }
		}

		public CreateFolderDialog()
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
			if (string.IsNullOrWhiteSpace(tbxFolderName.Text))
			{
				MessageBox.Show("Empty Folder name!");
				tbxFolderName.Focus();
				return;
			}

			DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tbxFolderName.SelectAll();
			tbxFolderName.Focus();
		}

		private void tbxFavoriteName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			OK();
		}
	}
}