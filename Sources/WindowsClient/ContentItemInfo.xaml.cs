using System;
using System.Collections.Generic;
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