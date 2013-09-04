using System;
#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Waveface.Client
{
	public partial class ZoomSlider : UserControl
	{
		#region Var

		public static readonly DependencyProperty _minimum = DependencyProperty.Register("Minimum", typeof(Double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, OnMinimumChanged));
		public static readonly DependencyProperty _maximum = DependencyProperty.Register("Maximum", typeof(Double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, OnMaximumChanged));
		public static readonly DependencyProperty _value = DependencyProperty.Register("Value", typeof(Double), typeof(ZoomSlider), new UIPropertyMetadata(0.0, OnValueChanged));

		#endregion

		#region Property

		public Double Minimum
		{
			get { return (Double)GetValue(_value); }
			set
			{
				SetValue(_value, value);
				slider.Minimum = value;
			}
		}

		public Double Maximum
		{
			get { return (Double)GetValue(_value); }
			set
			{
				SetValue(_value, value);
				slider.Maximum = value;
			}
		}

		public Double Value
		{
			get { return (Double)GetValue(_value); }
			set
			{
				SetValue(_value, value);
				slider.Value = value;
			}
		}

		#endregion

		public ZoomSlider()
		{
			InitializeComponent();
		}

		private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ZoomSlider;
			obj.Minimum = (Double)e.NewValue;
		}


		private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ZoomSlider;
			obj.Maximum = (Double)e.NewValue;
		}

		private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var obj = o as ZoomSlider;
			obj.Value = (Double)e.NewValue;
		}

		private void slider_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
		{
			Value = slider.Value;
		}
	}
}