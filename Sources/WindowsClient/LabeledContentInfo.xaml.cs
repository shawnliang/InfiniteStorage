using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for LabeledContentInfo.xaml
	/// </summary>
	public partial class LabeledContentInfo : UserControl
	{
		#region Var
		public static readonly DependencyProperty _photoCount = DependencyProperty.Register("PhotoCount", typeof(int), typeof(LabeledContentInfo), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPhotoCountChanged)));
		public static readonly DependencyProperty _videoCount = DependencyProperty.Register("VideoCount", typeof(int), typeof(LabeledContentInfo), new UIPropertyMetadata(0, new PropertyChangedCallback(OnVideoCountChanged)));
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
			}
		}
		#endregion

		public LabeledContentInfo()
		{
			this.InitializeComponent();
		}

		private static void OnPhotoCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var control = o as LabeledContentInfo;
			control.PhotoCount = (int)e.NewValue;
		}

		private static void OnVideoCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var control = o as LabeledContentInfo;
			control.VideoCount = (int)e.NewValue;
		}
	}
}