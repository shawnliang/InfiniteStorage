using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	public partial class Star : UserControl
	{
		#region Var
		public static readonly DependencyProperty _stared = DependencyProperty.Register("Stared", typeof(bool), typeof(Star), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStaredChanged)));
		#endregion

		#region Property
		public bool Stared
		{
			get
			{
				return (bool)GetValue(_stared);
			}
			set
			{
				SetValue(_stared, value);
				SelectedStar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		#endregion


		public Star()
		{
			this.InitializeComponent();
		}

		private static void OnStaredChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as Star;
			obj.Stared = (bool)e.NewValue;
		}
	}
}