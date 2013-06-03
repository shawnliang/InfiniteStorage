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
		public static readonly DependencyProperty _stared = DependencyProperty.Register("Stared", typeof(bool), typeof(ViewerControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStaredChanged)));
		public static readonly DependencyProperty _position = DependencyProperty.Register("Position", typeof(double), typeof(ViewerControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnPositionChanged)));
		public static readonly DependencyProperty _duration = DependencyProperty.Register("Duration", typeof(double), typeof(ViewerControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnDurationChanged)));
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

		public bool Stared
		{
			get
			{
				return (bool)GetValue(_stared);
			}
			set
			{
				SetValue(_stared, value);
				staredControl.Stared = value;
			}
		}

		public double Position
		{
			get
			{
				return (double)GetValue(_position);
			}
			set
			{
				SetValue(_position, value);
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
		#endregion

		#region Event
		public event EventHandler Previous;
		public event EventHandler Next;
		public event EventHandler Close;
		public event EventHandler PositionChanged;
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

		protected void OnPositionChanged(EventArgs e)
		{
			if (PositionChanged == null)
				return;
			PositionChanged(this, e);
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

		private static void OnStaredChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.Stared = (bool)e.NewValue;
		}

		private static void OnPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.Position = (double)e.NewValue;
		}


		private static void OnDurationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ViewerControl;
			obj.Duration = (double)e.NewValue;
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

		private void PlayProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OnPositionChanged(EventArgs.Empty);
		}
	}
}