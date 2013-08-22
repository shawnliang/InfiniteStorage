using System.Windows.Controls;
using System.Windows;
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
			"CoverWidth", typeof(int), typeof(AlbumItem), new PropertyMetadata(100));

		public int CoverWidth
		{
			get { return (int)GetValue(CoverWidthProperty); }
			set { SetValue(CoverWidthProperty, value); }
		}
		#endregion

		#region CoverHeight
		public static readonly DependencyProperty CoverHeightProperty = DependencyProperty.Register(
			"CoverHeight", typeof(int), typeof(AlbumItem), new PropertyMetadata(100));

		public int CoverHeight
		{
			get { return (int)GetValue(CoverHeightProperty); }
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
			"CoverBorderThickness", typeof(int), typeof(AlbumItem), new PropertyMetadata(4));

		public int CoverBorderThickness
		{
			get { return (int)GetValue(CoverBorderThicknessProperty); }
			set { SetValue(CoverBorderThicknessProperty, value); }
		}
		#endregion

		public AlbumItem()
		{
			this.InitializeComponent();
		}
	}
}