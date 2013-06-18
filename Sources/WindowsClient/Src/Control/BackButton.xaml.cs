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
		public static readonly DependencyProperty _enabled = DependencyProperty.Register("Enabled", typeof(bool), typeof(BackButton), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableChanged)));
		#endregion

		#region Property
		public bool Enabled
		{
			get
			{
				return (bool)GetValue(_enabled);
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
			obj.Enabled = (bool)e.NewValue;
		}
	}
}