using System;
#region

using System.Windows;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class CreateFolderDialog : Window
	{
		public String CreateName
		{
			get { return tbxFolderName.Text; }
			set { tbxFolderName.Text = value; }
		}

		public String DefaultName
		{
			get { return tbxFolderName.Text; }
			set { tbxFolderName.Text = value; }
		}

		public CreateFolderDialog()
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
			if (String.IsNullOrWhiteSpace(tbxFolderName.Text))
			{
				tbxFolderName.Focus();
				return;
			}

			DialogResult = true;
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			tbxFolderName.SelectAll();
			tbxFolderName.Focus();
		}

		private void tbxFavoriteName_KeyDown(Object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			OK();
		}
	}
}