using System.Collections;
using System.Windows;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for AddToFavoriteDialog.xaml
	/// </summary>
	public partial class AddToDialog : Window
	{
		public IEnumerable ItemSource
		{
			get
			{
				return cbxFavoriteName.ItemsSource;
			}
			set
			{
				cbxFavoriteName.ItemsSource = value;

				if (cbxFavoriteName.HasItems)
					cbxFavoriteName.SelectedIndex = 0;
			}
		}

		public object SelectedItem
		{
			get
			{
				return cbxFavoriteName.SelectedItem;
			}
			set
			{
				cbxFavoriteName.SelectedItem = value;
			}
		}

		public int SelectedIndex 
		{
			get
			{
				return cbxFavoriteName.SelectedIndex;
			}
			set
			{
				cbxFavoriteName.SelectedIndex = value;
			}
		}

		public AddToDialog()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cbxFavoriteName.Focus();
		}
	}
}