using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for FavoriteAllButton.xaml
	/// </summary>
	public partial class FavoriteAllButton : UserControl
	{
		public event RoutedEventHandler Click;
		public FavoriteAllButton()
		{
			this.InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var handler = Click;
			if (handler != null)
				handler(sender, e);
		}


	}
}