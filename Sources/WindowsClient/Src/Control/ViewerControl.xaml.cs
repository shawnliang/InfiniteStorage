#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class ViewerControl : UserControl
	{
		#region Var

		public static readonly DependencyProperty _pageNo = DependencyProperty.Register("PageNo", typeof(Int32), typeof(ViewerControl), new UIPropertyMetadata(0, OnPageNoChanged));

		public static readonly DependencyProperty _pageCount = DependencyProperty.Register("PageCount", typeof(Int32), typeof(ViewerControl),
																						   new UIPropertyMetadata(0, OnPageCountChanged));

		public static readonly DependencyProperty _enableStar = DependencyProperty.Register("EnableStar", typeof(Boolean), typeof(ViewerControl),
																							new UIPropertyMetadata(true, OnEnableStarChanged));

		public static readonly DependencyProperty _stared = DependencyProperty.Register("Stared", typeof(Boolean), typeof(ViewerControl), new UIPropertyMetadata(false, OnStaredChanged));

		#endregion

		#region Property

		public Int32 PageNo
		{
			get { return (Int32)GetValue(_pageNo); }
			set
			{
				SetValue(_pageNo, value);
				lblPageNo.Content = value;
			}
		}

		public Int32 PageCount
		{
			get { return (Int32)GetValue(_pageCount); }
			set
			{
				SetValue(_pageCount, value);
				lblPageCount.Content = value;
			}
		}

		public Boolean EnableStar
		{
			get { return (Boolean)GetValue(_enableStar); }
			set
			{
				SetValue(_enableStar, value);

				staredControl.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				image.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public Boolean Stared
		{
			get { return (Boolean)GetValue(_stared); }
			set { SetValue(_stared, value); }
		}

		#endregion

		#region Event

		public event EventHandler Previous;
		public event EventHandler Next;
		public event EventHandler Close;
		public event EventHandler DeletePic;

		#endregion

		public ViewerControl()
		{
			InitializeComponent();
		}

		#region Protected Method

		protected void OnPrevious(EventArgs e)
		{
			if (Previous == null)
				return;

			Previous(this, e);
		}

		protected void OnNext(EventArgs e)
		{
			if (Next == null)
				return;

			Next(this, e);
		}

		protected void OnClose(EventArgs e)
		{
			if (Close == null)
				return;

			Close(this, e);
		}

		#endregion

		private static void OnPageNoChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ViewerControl;
			obj.PageNo = (Int32)e.NewValue;
		}

		private static void OnPageCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ViewerControl;
			obj.PageCount = (Int32)e.NewValue;
		}

		private static void OnEnableStarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ViewerControl;
			obj.EnableStar = (Boolean)e.NewValue;
		}

		private static void OnStaredChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ViewerControl;
			obj.Stared = (Boolean)e.NewValue;
		}

		private void NextButton_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			OnNext(EventArgs.Empty);
		}

		private void PreviousButton_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			OnPrevious(EventArgs.Empty);
		}

		private void CloseButton_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			OnClose(EventArgs.Empty);
		}

		private void pvDelete_MouseDown(Object sender, MouseButtonEventArgs e)
		{
			if (DeletePic != null)
			{
				DeletePic(this, EventArgs.Empty);
			}
		}
	}
}