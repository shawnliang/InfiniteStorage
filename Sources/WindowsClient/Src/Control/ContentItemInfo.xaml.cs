using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for ContentItemInfo.xaml
	/// </summary>
	public partial class ContentItemInfo : UserControl
	{
		#region Var
		public static readonly DependencyProperty _fileName = DependencyProperty.Register("FileName", typeof(string), typeof(ContentItemInfo), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFileNameChanged)));
		#endregion

		#region Property
		public string FileName
		{
			get
			{
				return (string)GetValue(_fileName);
			}
			set
			{
				SetValue(_fileName, value);
				DateTaken.Text = value;
			}
		}
		#endregion

		public ContentItemInfo()
		{
			this.InitializeComponent();
		}

		private static void OnFileNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as ContentItemInfo;
			obj.FileName = (string)e.NewValue;
		}
	}
}