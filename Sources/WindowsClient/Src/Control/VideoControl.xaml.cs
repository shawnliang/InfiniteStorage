using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for VideoControl.xaml
	/// </summary>
	public partial class VideoControl : UserControl
	{
		public static readonly DependencyProperty _isPlaying = DependencyProperty.Register("IsPlaying", typeof(bool), typeof(VideoControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsPlayingChanged)));
		public static readonly DependencyProperty _duration = DependencyProperty.Register("Duration", typeof(double), typeof(VideoControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnDurationChanged)));

		public bool IsPlaying
		{
			get
			{
				return (bool)GetValue(_isPlaying);
			}
			set
			{
				SetValue(_isPlaying, value);
				pbPlay.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
				pbPause.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}


		public double Position
		{
			get
			{
				return PlayProgress.Value;
			}
			set
			{
				PlayProgress.Value = value;
			}
		}

		public double Duration
		{
			get
			{
				return (double)GetValue(_duration);
			}
			set
			{
				SetValue(_duration, value);
				PlayProgress.Maximum = value;
			}
		}

		public double Volume
		{
			get
			{
				return vcVolumeController.Volume;
			}
			set
			{
				vcVolumeController.Volume = value;
			}
		}



		#region Event
		public event EventHandler PlayButtonClick;
		public event EventHandler PauseButtonClick;
		public event EventHandler PositionChanged;
		public event EventHandler VolumeChanged;
		public event EventHandler SeekPosition;
		#endregion


		public VideoControl()
		{
			this.InitializeComponent();
		}

		#region Protected Method
		protected void OnVolumeChanged(EventArgs e)
		{
			if (VolumeChanged == null)
				return;
			VolumeChanged(this, e);
		}

		#endregion

		protected void OnPositionChanged(EventArgs e)
		{
			if (PositionChanged == null)
				return;
			PositionChanged(this, e);
		}

		protected void OnPlayButtonClick(EventArgs e)
		{
			if (PlayButtonClick == null)
				return;
			PlayButtonClick(this, e);
		}

		protected void OnPauseButtonClick(EventArgs e)
		{
			if (PauseButtonClick == null)
				return;
			PauseButtonClick(this, e);
		}

		protected void OnSeekPosition(EventArgs e)
		{
			if (SeekPosition == null)
				return;
			SeekPosition(this, e);
		}



		private void PlayProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
				OnSeekPosition(EventArgs.Empty);
			OnPositionChanged(EventArgs.Empty);
		}


		private static void OnDurationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as VideoControl;
			obj.Duration = (double)e.NewValue;
		}


		private static void OnIsPlayingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as VideoControl;
			obj.IsPlaying = (bool)e.NewValue;
		}


		private void PlayButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OnPlayButtonClick(EventArgs.Empty);
		}

		private void PauseButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OnPauseButtonClick(EventArgs.Empty);
		}

		private void vcVolumeController_VolumeChanged(object sender, EventArgs e)
		{
			OnVolumeChanged(EventArgs.Empty);
		}
	}
}