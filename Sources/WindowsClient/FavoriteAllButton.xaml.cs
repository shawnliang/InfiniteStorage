using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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