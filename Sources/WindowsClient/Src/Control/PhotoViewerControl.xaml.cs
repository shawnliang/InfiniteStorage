#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waveface.ClientFramework;
using Waveface.Model;

#endregion

namespace Waveface.Client
{
	public partial class PhotoViewerControl : UserControl
	{
		public event EventHandler Close;

		private DispatcherTimer m_timer;

		private ScaleTransform m_myScale = new ScaleTransform();
		private RotateTransform m_myRotate = new RotateTransform();
		private TransformGroup m_myTransformGroup = new TransformGroup();

		private double m_angle;

		#region Properties

		private DispatcherTimer Timer
		{
			get
			{
				if (m_timer == null)
				{
					m_timer = new DispatcherTimer
								  {
									  Interval = TimeSpan.FromMilliseconds(100)
								  };

					m_timer.Tick += timer_Tick;
				}

				return m_timer;
			}
		}

		public Int32 SelectedIndex
		{
			get { return lbImages.SelectedIndex; }
			set
			{
				lbImages.SelectedIndex = value;
				vcViewerControl.PageNo = value + 1;
			}
		}

		public Object SelectedItem
		{
			get { return lbImages.SelectedItem; }
			set { lbImages.SelectedItem = value; }
		}

		public Object Source
		{
			get { return lbImages.DataContext; }
			set
			{
				lbImages.DataContext = value;
				vcViewerControl.PageCount = (value as IEnumerable<IContentEntity>).Count();
			}
		}

		#endregion

		public PhotoViewerControl()
		{
			InitializeComponent();

			Observable.FromEvent<SelectionChangedEventHandler, SelectionChangedEventArgs>(
				handler => (s, ex) => handler(ex),
				h => lbImages.SelectionChanged += h,
				h => lbImages.SelectionChanged -= h)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.SubscribeOn(ThreadPoolScheduler.Instance)
				.ObserveOn(DispatcherScheduler.Current)
				.Subscribe(ex => TryDisplayOriginalPhoto());
		}

		private void WindowLoaded(Object sender, RoutedEventArgs e)
		{
			m_myTransformGroup.Children.Add(m_myScale);
			m_myTransformGroup.Children.Add(m_myRotate);

			ImgContentCtrl.RenderTransform = m_myTransformGroup;
			ImgContentCtrl.BorderBrush = Brushes.White;

			TryDisplayOriginalPhoto();
		}

		#region Close

		private void VC_Close(Object sender, EventArgs e)
		{
			OnClose(EventArgs.Empty);
		}

		protected void OnClose(EventArgs e)
		{
			if (Close == null)
				return;

			Close(this, e);
		}

		private void ImgContentCtrl_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left)
				return;

			OnClose(EventArgs.Empty);
		}

		#endregion

		#region Navigate

		public void Previous()
		{
			if (lbImages.Items.Count <= 0)
			{
				vcViewerControl.PageNo = 0;
				vcViewerControl.PageCount = 0;
				return;
			}

			var _value = lbImages.SelectedIndex - 1;

			if (_value < 0)
				_value = lbImages.Items.Count - 1;

			lbImages.SelectedIndex = _value;
			vcViewerControl.PageNo = lbImages.SelectedIndex + 1;
		}

		public void Next()
		{
			if (lbImages.Items.Count <= 0)
			{
				vcViewerControl.PageNo = 0;
				vcViewerControl.PageCount = 0;
				return;
			}

			lbImages.SelectedIndex = (lbImages.SelectedIndex + 1) % lbImages.Items.Count;
			vcViewerControl.PageNo = lbImages.SelectedIndex + 1;
		}

		private void lbImages_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			var _content = (lbImages.SelectedItem as BunnyContent);

			if (_content == null)
				return;

			ResetImgContentCtrlUI(true);

			if (_content.Type == ContentType.Video)
			{
				vcVideoControl.Visibility = Visibility.Visible;

				PlayVideo();
			}
			else
			{
				vcVideoControl.Visibility = Visibility.Collapsed;

				ImgObject.Source = _content.ThumbnailSource;
			}
		}

		private void TryDisplayOriginalPhoto()
		{
			vcViewerControl.ShowActualButton(false);

			var _content = (lbImages.SelectedItem as BunnyContent);

			if (_content == null)
				return;

			if (_content.Type != ContentType.Photo)
				return;

			//vcViewerControl.ShowActualButton(true);

			ImgObject.Source = _content.ImageSource;
		}

		#endregion

		#region ImgThumb

		private void ImgThumb_DragDelta(Object sender, DragDeltaEventArgs e)
		{
			Grid.SetRowSpan(ImgCanvas, 2);

			Double _left = Canvas.GetLeft(ImgContentCtrl);
			Double _top = Canvas.GetTop(ImgContentCtrl);

			double _dh = (e.HorizontalChange / 5);
			double _dv = (e.VerticalChange / 5);
			double _h = 0;
			double _v = 0;

			if (m_angle == 0)
			{
				_h = _dh;
				_v = _dv;
			}
			else if (m_angle == 90)
			{
				_h = _dv * -1;
				_v = _dh;
			}
			else if (m_angle == 180)
			{
				_h = _dh * -1;
				_v = _dv * -1;
			}
			else if (m_angle == 270)
			{
				_h = _dv;
				_v = _dh * -1;
			}

			_left += _h;
			_top += _v;

			Canvas.SetLeft(ImgContentCtrl, _left);
			Canvas.SetTop(ImgContentCtrl, _top);
		}

		private void ImgThumb_MouseWheel(Object sender, MouseWheelEventArgs e)
		{
			Grid.SetRowSpan(ImgCanvas, 2);

			int _deltaValue = e.Delta;

			if ((_deltaValue > 0))
			{
				if ((m_myScale.ScaleX < 4))
				{
					ZoomIn(0.025);
				}
			}
			else
			{
				if ((m_myScale.ScaleX > 0.5))
				{
					ZoomOut(0.025);
				}
			}
		}

		#endregion

		public void ZoomOut(double r = 0.1)
		{
			m_myScale.ScaleX -= r;
			m_myScale.ScaleY = m_myScale.ScaleX;
		}

		public void ZoomIn(double r = 0.1)
		{
			m_myScale.ScaleX += r;
			m_myScale.ScaleY = m_myScale.ScaleX;
		}

		private void ResetImgContentCtrlUI(bool resetAngle)
		{
			m_myScale.ScaleX = 1;
			m_myScale.ScaleY = 1;

			Canvas.SetLeft(ImgContentCtrl, 0);
			Canvas.SetTop(ImgContentCtrl, 0);

			Grid.SetRowSpan(ImgCanvas, 1);

			if (resetAngle)
			{
				m_myRotate.Angle = 0;
				m_angle = 0;
			}		
		}

		private void VC_OnDeletePic(Object sender, EventArgs e)
		{
			Int32 _index = lbImages.SelectedIndex;

			IContentEntity _contentEntity = (IContentEntity)lbImages.SelectedItem;

			if (MessageBox.Show(Application.Current.MainWindow, "Are you sure you want to delete?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
				return;

			var parentWindow = Tag as Window;
			MainWindow _mainWindow = (MainWindow)parentWindow.Owner;

			_mainWindow.DeleteSourceContents(new[] { _contentEntity.ID }, false);

			if (lbImages.Items.Count > _index)
			{
				lbImages.SelectedIndex = _index;
			}
			else
			{
				if (lbImages.Items.Count > 0)
				{
					lbImages.SelectedIndex = 0;
				}
				else
				{
					vcViewerControl.PageNo = 0;
					vcViewerControl.PageCount = 0;
				}
			}

			vcViewerControl.PageNo = lbImages.SelectedIndex + 1;
			vcViewerControl.PageCount = lbImages.Items.Count;

			parentWindow.Activate();
		}

		#region Video

		private void timer_Tick(Object sender, EventArgs e)
		{
			vcVideoControl.Position = meVideo.Position.TotalMilliseconds;
		}

		private void StopVideo()
		{
			Timer.Stop();
			meVideo.Stop();

			vcVideoControl.IsPlaying = false;
		}

		private void PlayVideo()
		{
			Timer.Start();
			meVideo.Play();

			vcVideoControl.IsPlaying = true;
		}

		private void PauseVideo()
		{
			meVideo.Pause();

			vcVideoControl.IsPlaying = false;
		}

		private void meVideo_MediaOpened(Object sender, RoutedEventArgs e)
		{
			vcVideoControl.Duration = meVideo.NaturalDuration.TimeSpan.TotalMilliseconds;
			vcVideoControl.Volume = meVideo.Volume;
		}

		private void meVideo_MediaEnded(Object sender, RoutedEventArgs e)
		{
			StopVideo();

			meVideo.Position = TimeSpan.FromMilliseconds(0);
		}

		private void meVideo_MediaFailed(Object sender, ExceptionRoutedEventArgs e)
		{
			StopVideo();

			meVideo.Position = TimeSpan.FromMilliseconds(0);
		}

		private void vcVideoControl_PlayButtonClick(Object sender, EventArgs e)
		{
			PlayVideo();
		}

		private void vcVideoControl_VolumeChanged(Object sender, EventArgs e)
		{
			meVideo.Volume = vcVideoControl.Volume;
		}

		private void vcVideoControl_SeekPosition(Object sender, EventArgs e)
		{
			PauseVideo();

			meVideo.Position = TimeSpan.FromMilliseconds(vcVideoControl.Position);

			PlayVideo();
		}

		private void vcVideoControl_PauseButtonClick(Object sender, EventArgs e)
		{
			PauseVideo();
		}

		#endregion

		#region Button Click

		private void VC_Next(Object sender, EventArgs e)
		{
			Next();
		}

		private void VC_Previous(Object sender, EventArgs e)
		{
			Previous();
		}

		private void VC_FitScreen(object sender, EventArgs e)
		{
			ResetImgContentCtrlUI(false);
		}

		private void VC_TurnCCW(object sender, EventArgs e)
		{
			ResetImgContentCtrlUI(false);

			if (m_angle == 0)
			{
				m_angle = 360;
			}

			m_angle -= 90;

			m_myRotate.Angle = m_angle;
		}

		private void VC_TurnCW(object sender, EventArgs e)
		{
			ResetImgContentCtrlUI(false);

			m_angle += 90;

			if (m_angle == 360)
			{
				m_angle = 0;
			}

			m_myRotate.Angle = m_angle;
		}

		private void VC_ActualSize(object sender, EventArgs e)
		{

		}

		private void VC_ZoomIn(object sender, EventArgs e)
		{
			ZoomIn();
		}

		private void VC_ZoomOut(object sender, EventArgs e)
		{
			ZoomOut();
		}

		#endregion
	}
}