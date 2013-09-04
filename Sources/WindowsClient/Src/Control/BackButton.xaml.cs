using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for BackButton.xaml
	/// </summary>
	public partial class BackButton : Button
	{
		#region Var
		public static readonly DependencyProperty _enabled = DependencyProperty.Register("Enabled", typeof(Boolean), typeof(BackButton), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableChanged)));
		#endregion

		#region Property
		public Boolean Enabled
		{
			get
			{
				return (Boolean)GetValue(_enabled);
			}
			set
			{
				SetValue(_enabled, value);
			}
		}
		#endregion

		public BackButton()
		{
			this.InitializeComponent();
		}

		private static void OnEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as BackButton;
			obj.Enabled = (Boolean)e.NewValue;
		}
	}
}