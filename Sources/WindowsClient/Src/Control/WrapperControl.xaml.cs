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
using System.Reflection;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for WrapperControl.xaml
	/// </summary>
	public partial class WrapperControl : UserControl
	{
		#region Var
		public static readonly DependencyProperty _type = DependencyProperty.Register("Type", typeof(Type), typeof(WrapperControl), new UIPropertyMetadata(null, null));
		#endregion

		#region Property
		private Boolean m_IsInited { get; set; }

		public Type Type
		{
			get
			{
				return (Type)GetValue(_type);
			}
			set
			{
				SetValue(_type, value);
			}
		}
		#endregion

		public WrapperControl()
		{
			InitializeComponent();
		}

		public void Display()
		{
			if (m_IsInited)
				return;

			if (Type == null)
				return;

			var element = Activator.CreateInstance(Type) as FrameworkElement;

			this.Content = element;

			m_IsInited = true;
		}
	}
}
