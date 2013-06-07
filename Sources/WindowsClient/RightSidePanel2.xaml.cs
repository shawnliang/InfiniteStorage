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
	/// Interaction logic for RightSidePanel2.xaml
	/// </summary>
	public partial class RightSidePanel2 : UserControl
	{
		#region Var
		public static readonly DependencyProperty _favoriteName = DependencyProperty.Register("FavoriteName", typeof(string), typeof(RightSidePanel2), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFavoriteNameChanged)));
		#endregion

		#region Property
		public string FavoriteName
		{
			get
			{
				return (string)GetValue(_favoriteName);
			}
			set
			{
				SetValue(_favoriteName, value);
				lblName.Content = value;
			}
		}
		#endregion

		#region Event
		public event EventHandler OnAirClick;
		#endregion
		
		
		public RightSidePanel2()
		{
			this.InitializeComponent();
		}
		
		#region Protected Method
		protected void OnOnAirClick(EventArgs e)
		{
			if(OnAirClick == null)
				return;
			OnAirClick(this, e);
		}
		#endregion

		private static void OnFavoriteNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var control = o as RightSidePanel2;
			control.FavoriteName = (string)e.NewValue;
		}

		private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnOnAirClick(EventArgs.Empty);
		}

		private void btnAction_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!btnAction.IsEnabled)
				btnAction.Content = "Disabled";
		}

		private void btnAction_Unchecked(object sender, RoutedEventArgs e)
		{
			btnAction.Content = "Offline";
		}

		private void btnAction_Checked(object sender, RoutedEventArgs e)
		{
			btnAction.Content = "On Air";
		}
	}
}