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
	public partial class IntroStep3 : Window
	{
		public event EventHandler OpenNowButtonClicked;

		public IntroStep3()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();

			var handler = OpenNowButtonClicked;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
