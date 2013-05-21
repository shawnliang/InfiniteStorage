using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for PhotoViewerControl.xaml
	/// </summary>
	public partial class PhotoViewerControl : UserControl
	{
		ScaleTransform myScale = new ScaleTransform();


		public PhotoViewerControl()
		{
			InitializeComponent();
		}


		public int SelectedIndex
		{
			get
			{
				return lbImages.SelectedIndex;
			}
			set
			{
				lbImages.SelectedIndex = value;
			}
		}

		public Object Source
		{
			get
			{
				return lbImages.DataContext;
			}
			set
			{
				lbImages.DataContext = value;
			}
		}


		public void Previous()
		{
			var value = lbImages.SelectedIndex - 1;

			if (value < 0)
				value = lbImages.Items.Count - 1;

			lbImages.SelectedIndex = value;
		}

		public void Next()
		{
			lbImages.SelectedIndex = (lbImages.SelectedIndex + 1) % lbImages.Items.Count;
		}


		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			ImgContentCtrl.RenderTransform = myScale;
			ImgContentCtrl.BorderBrush = Brushes.White;
		}



		private void ImgThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			double left = Canvas.GetLeft(ImgContentCtrl);
			double top = Canvas.GetTop(ImgContentCtrl);

			left += e.HorizontalChange;
			top += e.VerticalChange;

			Canvas.SetLeft(ImgContentCtrl, left);
			Canvas.SetTop(ImgContentCtrl, top);
		}


		private void ImgThumb_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			//  Variable for holding the mouse's delta value.
			int deltaValue;
			deltaValue = e.Delta;
			//  Set the center point of the ScaleTransform object
			//  to the cursor location.
			myScale.CenterX = e.GetPosition(ImgContentCtrl).X;
			myScale.CenterY = e.GetPosition(ImgContentCtrl).Y;
			//  Zoom in when the user scrolls the mouse wheel up
			//  and vice versa.
			if ((deltaValue > 0))
			{
				//  Limit zoom-in to 500%
				if ((myScale.ScaleX < 5))
				{
					ZoomIn();
				}
				//  When mouse wheel is scrolled down...
			}
			else
			{
				//  Limit zoom-out to 80%
				if ((myScale.ScaleX > 0.8))
				{
					ZoomOut();
				}
			}
		}

		public void ZoomOut()
		{
			//  Zoom-out by 10%
			myScale.ScaleX -= 0.1;
			myScale.ScaleY = myScale.ScaleX;
		}

		public void ZoomIn()
		{
			//  Zoom-in in 10% increments
			myScale.ScaleX += 0.1;
			myScale.ScaleY = myScale.ScaleX;
		}
	}
}