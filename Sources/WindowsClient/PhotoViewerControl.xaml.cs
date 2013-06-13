using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waveface.Model;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for PhotoViewerControl.xaml
	/// </summary>
	public partial class PhotoViewerControl : UserControl
	{
		DispatcherTimer _timer;
		ScaleTransform myScale = new ScaleTransform();


		private DispatcherTimer m_Timer 
		{
			get
			{
				if (_timer == null)
				{
					_timer = new DispatcherTimer();
					_timer.Interval = TimeSpan.FromMilliseconds(100);
					_timer.Tick += _timer_Tick;
				}
				return _timer;
			}
		}

		public event EventHandler Close;


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
				vcViewerControl.PageNo = value + 1;
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
				vcViewerControl.PageCount = (value as IEnumerable<IContentEntity>).Count();
			}
		}


		protected void OnClose(EventArgs e)
		{
			if (Close == null)
				return;
			Close(this, e);
		}


		public void Previous()
		{
			var value = lbImages.SelectedIndex - 1;

			if (value < 0)
				value = lbImages.Items.Count - 1;

			lbImages.SelectedIndex = value;
			vcViewerControl.PageNo = lbImages.SelectedIndex + 1;
		}

		public void Next()
		{
			lbImages.SelectedIndex = (lbImages.SelectedIndex + 1) % lbImages.Items.Count;
			vcViewerControl.PageNo = lbImages.SelectedIndex + 1;
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

		private void ViewerControl_Next(object sender, EventArgs e)
		{
			Next();
		}

		private void ViewerControl_Previous(object sender, EventArgs e)
		{
			Previous();
		}

		private void ViewerControl_Close(object sender, EventArgs e)
		{
			OnClose(EventArgs.Empty);
		}

		private void ImgContentCtrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			OnClose(EventArgs.Empty);
		}

		private void meVideo_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
		{
			vcVideoControl.Duration = meVideo.NaturalDuration.TimeSpan.TotalMilliseconds;
			vcVideoControl.Volume = meVideo.Volume;
		}

		void _timer_Tick(object sender, EventArgs e)
		{
			vcVideoControl.Position = meVideo.Position.TotalMilliseconds;
			
		}

		private void meVideo_MediaEnded(object sender, RoutedEventArgs e)
		{
			StopVideo();
			meVideo.Position = TimeSpan.FromMilliseconds(0);
		}

		private void StopVideo()
		{
			m_Timer.Stop();
			meVideo.Stop();
			vcVideoControl.IsPlaying = false;
		}

		private void meVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			StopVideo();
			meVideo.Position = TimeSpan.FromMilliseconds(0);
		}

		private void vcVideoControl_PlayButtonClick(object sender, System.EventArgs e)
		{
			PlayVideo();
		}

		private void PlayVideo()
		{
			m_Timer.Start();
			meVideo.Play();
			vcVideoControl.IsPlaying = true;
		}

		private void vcVideoControl_PauseButtonClick(object sender, System.EventArgs e)
		{
			PauseVideo();
		}

		private void PauseVideo()
		{
			meVideo.Pause();
			vcVideoControl.IsPlaying = false;
		}

		private void lbImages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var content = (lbImages.SelectedItem as IContent);

			if (content == null)
				return;

			if (content.Type == ContentType.Video)
			{
				vcVideoControl.Visibility = Visibility.Visible;
				PlayVideo();
			}
			else 
			{
				vcVideoControl.Visibility = Visibility.Collapsed;
			}
		}

		private void vcVideoControl_VolumeChanged(object sender, System.EventArgs e)
		{
			meVideo.Volume = vcVideoControl.Volume;
		}

		private void vcVideoControl_SeekPosition(object sender, System.EventArgs e)
		{
			
			PauseVideo();
			meVideo.Position = TimeSpan.FromMilliseconds(vcVideoControl.Position);
			PlayVideo();
		}

	}
}