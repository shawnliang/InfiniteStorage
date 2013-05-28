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
		public static readonly DependencyProperty _photoCount = DependencyProperty.Register("PhotoCount", typeof(int), typeof(LabeledContentInfo), new UIPropertyMetadata(0, null));
		public static readonly DependencyProperty _videoCount = DependencyProperty.Register("VideoCount", typeof(int), typeof(LabeledContentInfo), new UIPropertyMetadata(0, null));
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
	}
}