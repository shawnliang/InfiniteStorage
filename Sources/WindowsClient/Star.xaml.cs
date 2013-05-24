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