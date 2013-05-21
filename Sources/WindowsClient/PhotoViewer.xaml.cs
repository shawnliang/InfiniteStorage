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
	/// Interaction logic for PhotoViewer.xaml
	/// </summary>
	public partial class PhotoViewer : Window
	{
		public PhotoViewer()
		{
			InitializeComponent();
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			this.Close();
		}


		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
				case Key.Back:
					this.Close();
					break;
				case Key.Left:
					pvcViewer.Previous();
					break;
 				case Key.Right:
					pvcViewer.Next();
					break;
				case Key.OemPlus:
					pvcViewer.ZoomIn();
					break;
				case Key.OemMinus:
					pvcViewer.ZoomOut();
					break;
			}
		}

		private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}
	}
}
