using System;
#region

using System.Collections;
using System.Windows;

#endregion

namespace Waveface.Client
{
	public partial class MoveToFolderDialog : Window
	{
		public IEnumerable ItemSource
		{
			get { return cbxFolderName.ItemsSource; }
			set
			{
				cbxFolderName.ItemsSource = value;

				if (cbxFolderName.HasItems)
					cbxFolderName.SelectedIndex = 0;
			}
		}

		public Object SelectedItem
		{
			get { return cbxFolderName.SelectedItem; }
			set { cbxFolderName.SelectedItem = value; }
		}

		public Int32 SelectedIndex
		{
			get { return cbxFolderName.SelectedIndex; }
			set { cbxFolderName.SelectedIndex = value; }
		}

		public MoveToFolderDialog()
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
			cbxFolderName.Focus();
		}
	}
}