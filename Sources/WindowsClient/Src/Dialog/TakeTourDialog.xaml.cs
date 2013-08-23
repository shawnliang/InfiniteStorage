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

		public bool? ShowDialog(string resourceKey)
		{
			msgBox.SetResourceReference(TextBlock.TextProperty, resourceKey);
			return ShowDialog();
		}

		private void takeTourButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}

		private void laterButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}

		public static bool? ShowWithDynamicResource(string resourceKey, Window owner)
		{
			var dialog = new TakeTourDialog();
			dialog.Owner = owner;
			return dialog.ShowDialog(resourceKey);
		}
	}
}
