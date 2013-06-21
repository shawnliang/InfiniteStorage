using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for FavoriteListBox.xaml
	/// </summary>
	public partial class FavoriteListBox : ListBox
	{
		public FavoriteListBox()
		{
			this.InitializeComponent();
		}

		private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			e.Handled = true;
		}
	}
}