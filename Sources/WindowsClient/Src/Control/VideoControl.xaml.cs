#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class VideoControl : UserControl
	{
		public static readonly DependencyProperty _isPlaying = DependencyProperty.Register("IsPlaying", typeof (Boolean), typeof (VideoControl),
		                                                                                   new UIPropertyMetadata(false, OnIsPlayingChanged));

		public static readonly DependencyProperty _duration = DependencyProperty.Register("Duration", typeof (Double), typeof (VideoControl),
		                                                                                  new UIPropertyMetadata(0.0, OnDurationChanged));

		public Boolean IsPlaying
		{
			get { return (Boolean) GetValue(_isPlaying); }
			set
			{
				SetValue(_isPlaying, value);
				pbPlay.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
				pbPause.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Double Position
		{
			get { return PlayProgress.Value; }
			set { PlayProgress.Value = value; }
		}

		public Double Duration
		{
			get { return (Double) GetValue(_duration); }
			set
			{
				SetValue(_duration, value);
				PlayProgress.Maximum = value;
			}
		}

		public Double Volume
		{
			get { return vcVolumeController.Volume; }
			set { vcVolumeController.Volume = value; }
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
			InitializeComponent();
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

		private void PlayProgress_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
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
			obj.Duration = (Double) e.NewValue;
		}

		private static void OnIsPlayingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as VideoControl;
			obj.IsPlaying = (Boolean) e.NewValue;
		}

		private void PlayButton_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			OnPlayButtonClick(EventArgs.Empty);
		}

		private void PauseButton_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			OnPauseButtonClick(EventArgs.Empty);
		}

		private void vcVolumeController_VolumeChanged(Object sender, EventArgs e)
		{
			OnVolumeChanged(EventArgs.Empty);
		}
	}
}