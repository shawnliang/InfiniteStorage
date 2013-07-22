using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	public partial class FavoriteListBox : ListBox
	{
		public event EventHandler DeleteFavoriteInvoked;

		public FavoriteListBox()
		{
			InitializeComponent();
		}

		protected void OnDeleteFavoriteInvoked(EventArgs e)
		{
			if (DeleteFavoriteInvoked == null)
				return;

			DeleteFavoriteInvoked(this, e);
		}

		private void UserControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			e.Handled = true;
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OnDeleteFavoriteInvoked(EventArgs.Empty);
		}
	}
}