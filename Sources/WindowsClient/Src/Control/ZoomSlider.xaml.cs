using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for ZoomSlider.xaml
	/// </summary>
	public partial class ZoomSlider : UserControl
	{
		#region Var
		public static readonly DependencyProperty _minimum = DependencyProperty.Register("Minimum", typeof(double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinimumChanged)));
		public static readonly DependencyProperty _maximum = DependencyProperty.Register("Maximum", typeof(double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMaximumChanged)));
		public static readonly DependencyProperty _value = DependencyProperty.Register("Value", typeof(double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));
		#endregion

		#region Property
		public double Minimum
		{
			get
			{
				return (double)GetValue(_value);
			}
			set
			{
				SetValue(_value, value);
				slider.Minimum = value;
			}
		}

		public double Maximum
		{
			get
			{
				return (double)GetValue(_value);
			}
			set
			{
				SetValue(_value, value);
				slider.Maximum = value;
			}
		}

		public double Value
		{
			get
			{
				return (double)GetValue(_value);
			}
			set
			{
				SetValue(_value, value);
				slider.Value = value;
			}
		}
		#endregion

		public ZoomSlider()
		{
			this.InitializeComponent();
		}

		private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ZoomSlider;
			obj.Minimum = (double)e.NewValue;
		}


		private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ZoomSlider;
			obj.Maximum = (double)e.NewValue;
		}

		private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ZoomSlider;
			obj.Value = (double)e.NewValue;
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			this.Value = slider.Value;
		}
	}
}