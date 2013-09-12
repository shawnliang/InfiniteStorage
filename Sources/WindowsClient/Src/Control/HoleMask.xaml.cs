using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// HoleMask.xaml 的互動邏輯
	/// </summary>
	public partial class HoleMask : UserControl
	{
		public static readonly DependencyProperty HoleXYProperty = DependencyProperty.Register("HoleXY", typeof(Point), typeof(HoleMask), new PropertyMetadata(new Point()));
		public static readonly DependencyProperty HoleWidthProperty = DependencyProperty.Register("HoleWidth", typeof(int), typeof(HoleMask), new PropertyMetadata(20));
		public static readonly DependencyProperty HoleHeightProperty = DependencyProperty.Register("HoleHeight", typeof(int), typeof(HoleMask), new PropertyMetadata(20));

		public Point HoleXY
		{
			get { return (Point)GetValue(HoleXYProperty); }
			set { SetValue(HoleXYProperty, value); }
		}

		public int HoleWidth
		{
			get { return (int)GetValue(HoleWidthProperty); }
			set { SetValue(HoleWidthProperty, value); }
		}

		public int HoleHeight
		{
			get { return (int)GetValue(HoleHeightProperty); }
			set { SetValue(HoleHeightProperty, value); }
		}

		public HoleMask()
		{
			InitializeComponent();
		}
	}
}
