﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for RightSidePanel1.xaml
	/// </summary>
	public partial class RightSidePanel1 : UserControl
	{
		#region Var
		public static readonly DependencyProperty _photoCount = DependencyProperty.Register("PhotoCount", typeof(int), typeof(RightSidePanel1), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPhotoCountChanged)));
		public static readonly DependencyProperty _videoCount = DependencyProperty.Register("VideoCount", typeof(int), typeof(RightSidePanel1), new UIPropertyMetadata(0, new PropertyChangedCallback(OnVideoCountChanged)));
		#endregion

		#region Property
		public int PhotoCount
		{
			get
			{
				return (int)GetValue(_photoCount);
			}
			set
			{
				SetValue(_photoCount, value);
				LabeledCount.PhotoCount = value;
			}
		}

		public int VideoCount
		{
			get
			{
				return (int)GetValue(_videoCount);
			}
			set
			{
				SetValue(_videoCount, value);
				LabeledCount.VideoCount = value;
			}
		}
		#endregion


		#region Event
		public event EventHandler AddToFavorite;
		public event EventHandler SaveToFavorite;
		#endregion


		public RightSidePanel1()
		{
			this.InitializeComponent();
		}


		#region Protected Method
		protected void OnAddToFavorite(EventArgs e)
		{
			if (AddToFavorite == null)
				return;
			AddToFavorite(this, e);
		}

		protected void OnSaveToFavorite(EventArgs e)
		{
			if (SaveToFavorite == null)
				return;
			SaveToFavorite(this, e);
		}
		#endregion


		private static void OnPhotoCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var control = o as RightSidePanel1;
			control.PhotoCount = (int)e.NewValue;
		}

		private static void OnVideoCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var control = o as RightSidePanel1;
			control.VideoCount = (int)e.NewValue;
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnAddToFavorite(EventArgs.Empty);
		}

		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			OnSaveToFavorite(EventArgs.Empty);
		}
	}
}