using System.Collections;
using System.Windows;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for AddToFavoriteDialog.xaml
	/// </summary>
	public partial class AddToFavoriteDialog : Window
	{
		public IEnumerable FavoriteItemSource
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

		public object SelectedFavorite
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

		public AddToFavoriteDialog()
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