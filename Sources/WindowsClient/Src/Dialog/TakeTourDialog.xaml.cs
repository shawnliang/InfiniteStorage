using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for TakeTourDialog.xaml
	/// </summary>
	public partial class TakeTourDialog : Window
	{
		public TakeTourDialog()
		{
			InitializeComponent();
		}

		public Boolean? ShowDialog(String resourceKey)
		{
			msgBox.SetResourceReference(TextBlock.TextProperty, resourceKey);
			return ShowDialog();
		}

		private void takeTourButton_Click(Object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}

		private void laterButton_Click(Object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}

		public static Boolean? ShowWithDynamicResource(String resourceKey, Window owner)
		{
			var dialog = new TakeTourDialog();
			dialog.Owner = owner;
			return dialog.ShowDialog(resourceKey);
		}
	}
}
