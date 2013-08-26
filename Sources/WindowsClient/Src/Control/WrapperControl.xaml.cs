using System;
using System.Windows;
using System.Windows.Controls;

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

		#region Constructor
		public WrapperControl()
		{
			InitializeComponent();
		} 
		#endregion

		#region Public Method
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
		#endregion
	}
}
