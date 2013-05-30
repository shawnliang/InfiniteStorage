using System;
using System.Windows;
using System.Windows.Input;

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
			//if (e.ChangedButton != MouseButton.Left)
			//	return;

			//this.Close();
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

		private void pvcViewer_Close(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
