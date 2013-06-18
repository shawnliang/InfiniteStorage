using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for ViewerControl.xaml
	/// </summary>
	public partial class ViewerControl : UserControl
	{
		#region Var
		public static readonly DependencyProperty _pageNo = DependencyProperty.Register("PageNo", typeof(int), typeof(ViewerControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPageNoChanged)));
		public static readonly DependencyProperty _pageCount = DependencyProperty.Register("PageCount", typeof(int), typeof(ViewerControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPageCountChanged)));
		public static readonly DependencyProperty _enableStar = DependencyProperty.Register("EnableStar", typeof(bool), typeof(ViewerControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableStarChanged)));
		public static readonly DependencyProperty _stared = DependencyProperty.Register("Stared", typeof(bool), typeof(ViewerControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStaredChanged)));
		#endregion

		#region Property
		public int PageNo
		{
			get
			{
				return (int)GetValue(_pageNo);
			}
			set
			{
				SetValue(_pageNo, value);
				lblPageNo.Content = value;
			}
		}

		public int PageCount
		{
			get
			{
				return (int)GetValue(_pageCount);
			}
			set
			{
				SetValue(_pageCount, value);
				lblPageCount.Content = value;
			}
		}

		public bool EnableStar
		{
			get
			{
				return (bool)GetValue(_enableStar);
			}
			set
			{
				SetValue(_enableStar, value);
				staredControl.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool Stared
		{
			get
			{
				return (bool)GetValue(_stared);
			}
			set
			{
				SetValue(_stared, value);
				staredControl.Tagged = value;
			}
		}
		#endregion

		#region Event
		public event EventHandler Previous;
		public event EventHandler Next;
		public event EventHandler Close;
		#endregion

		public ViewerControl()
		{
			this.InitializeComponent();
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
			obj.PageNo = (int)e.NewValue;
		}

		private static void OnPageCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.PageCount = (int)e.NewValue;
		}



		private static void OnEnableStarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.EnableStar = (bool)e.NewValue;
		}


		private static void OnStaredChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.Stared = (bool)e.NewValue;
		}




		private void NextButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OnNext(EventArgs.Empty);
		}

		private void PreviousButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnPrevious(EventArgs.Empty);
		}

		private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OnClose(EventArgs.Empty);
		}


		private void staredControl_MouseDown_1(object sender, MouseButtonEventArgs e)
		{
			this.Stared = !this.Stared;
		}
	}
}