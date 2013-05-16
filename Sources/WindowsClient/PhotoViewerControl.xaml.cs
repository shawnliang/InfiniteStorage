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


		public Object SelectedSource
		{
			get
			{
				return ImgObject.DataContext;
			}
			set
			{
				ImgObject.DataContext = value;
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
					//  Zoom-in in 10% increments
					myScale.ScaleX += 0.1;
				}
				//  When mouse wheel is scrolled down...
			}
			else
			{
				//  Limit zoom-out to 80%
				if ((myScale.ScaleX > 0.8))
				{
					//  Zoom-out by 10%
					myScale.ScaleX -= 0.1;
				}
			}
			myScale.ScaleY = myScale.ScaleX;
		}


		private void lbImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{	//if (lbImages.SelectedIndex == -1)
			//	return;

			//this.SelectedPhoto = (Model.ContentItem)lbImages.SelectedItem;
		}
	}
}