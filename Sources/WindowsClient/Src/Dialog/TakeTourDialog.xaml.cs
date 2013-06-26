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
	/// Interaction logic for TakeTourDialog.xaml
	/// </summary>
	public partial class TakeTourDialog : Window
	{
		public TakeTourDialog()
		{
			InitializeComponent();
		}

		public bool? ShowDialog(string msg)
		{
			msgBox.Text = msg;
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

		public static bool? Show(string msg, Window owner)
		{
			var dialog = new TakeTourDialog();
			dialog.Owner = owner;
			return dialog.ShowDialog(msg);
		}
	}
}
