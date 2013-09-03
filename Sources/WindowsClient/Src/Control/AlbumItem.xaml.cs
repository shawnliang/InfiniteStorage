using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for ContentItem.xaml
	/// </summary>
	public partial class AlbumItem : UserControl
	{
		#region CoverWidth
		public static readonly DependencyProperty CoverWidthProperty = DependencyProperty.Register(
			"CoverWidth", typeof(Int32), typeof(AlbumItem), new PropertyMetadata(100));

		public Int32 CoverWidth
		{
			get { return (Int32)GetValue(CoverWidthProperty); }
			set { SetValue(CoverWidthProperty, value); }
		}
		#endregion

		#region CoverHeight
		public static readonly DependencyProperty CoverHeightProperty = DependencyProperty.Register(
			"CoverHeight", typeof(Int32), typeof(AlbumItem), new PropertyMetadata(100));

		public Int32 CoverHeight
		{
			get { return (Int32)GetValue(CoverHeightProperty); }
			set { SetValue(CoverHeightProperty, value); }
		}
		#endregion

		#region CoverBorderBrush
		public static readonly DependencyProperty CoverBorderBrushProperty = DependencyProperty.Register(
			"CoverBorderBrush", typeof(Brush), typeof(AlbumItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

		public Brush CoverBorderBrush
		{
			get { return (Brush)GetValue(CoverBorderBrushProperty); }
			set { SetValue(CoverBorderBrushProperty, value); }
		}
		#endregion

		#region CoverBorderThickness
		public static readonly DependencyProperty CoverBorderThicknessProperty = DependencyProperty.Register(
			"CoverBorderThickness", typeof(Int32), typeof(AlbumItem), new PropertyMetadata(4));

		public Int32 CoverBorderThickness
		{
			get { return (Int32)GetValue(CoverBorderThicknessProperty); }
			set { SetValue(CoverBorderThicknessProperty, value); }
		}
		#endregion

		public AlbumItem()
		{
			this.InitializeComponent();
		}
	}
}