using System.Windows;
using System.Windows.Input;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for CreateFavoriteDialog.xaml
	/// </summary>
	public partial class CreateFavoriteDialog : Window
	{
		public string FavoriteName
		{
			get
			{
				return tbxFavoriteName.Text;
			}
			set
			{
				tbxFavoriteName.Text = value;
			}
		}

		public CreateFavoriteDialog()
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
			tbxFavoriteName.SelectAll();
			tbxFavoriteName.Focus();
		}

		private void tbxFavoriteName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			this.DialogResult = true;
		}
	}
}