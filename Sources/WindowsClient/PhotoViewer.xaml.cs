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
		public int SelectedIndex
		{
			get
			{
				return pvcViewer.SelectedIndex;
			}
			set
			{
				pvcViewer.SelectedIndex = value;
			}
		}

		public Object Source
		{
			get
			{
				return pvcViewer.Source;
			}
			set
			{
				pvcViewer.Source = value;
			}
		}


		public PhotoViewer()
		{
			InitializeComponent();
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
