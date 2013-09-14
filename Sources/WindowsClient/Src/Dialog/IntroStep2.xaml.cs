using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// IntroStep3.xaml 的互動邏輯
	/// </summary>
	public partial class IntroStep2 : Window
	{
		public IntroStep2()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();

			var main = (MainWindow)this.Owner;
			main.CreateCloudAlbum(true);
		}

		private void IntroLayout_CloseButtomClicked(object sender, EventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
