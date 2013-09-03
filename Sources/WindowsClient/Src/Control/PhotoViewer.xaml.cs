#region

using System;
using System.Windows;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class PhotoViewer : Window
	{
		public Int32 SelectedIndex
		{
			get { return pvcViewer.SelectedIndex; }
			set { pvcViewer.SelectedIndex = value; }
		}

		public Object SelectedItem
		{
			get { return pvcViewer.SelectedItem; }
			set { pvcViewer.SelectedItem = value; }
		}

		public Object Source
		{
			get { return pvcViewer.Source; }
			set { pvcViewer.Source = value; }
		}

		public PhotoViewer()
		{
			InitializeComponent();
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			pvcViewer.Tag = this; //TODO: 重構
		}

		private void Window_KeyDown(Object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
				case Key.Back:
					Close();
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

		private void Window_MouseRightButtonUp(Object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		private void pvcViewer_Close(Object sender, EventArgs e)
		{
			Close();
		}
	}
}