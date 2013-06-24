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
	/// Interaction logic for SwitchButton.xaml
	/// </summary>
	public partial class SwitchButton : UserControl
	{
		#region Var
		public static readonly DependencyProperty _isOn = DependencyProperty.Register("IsOn", typeof(bool), typeof(SwitchButton), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsOnChanged)));
		#endregion

		#region Property
		public bool IsOn
		{
			get
			{
				return (bool)GetValue(_isOn);
			}
			set
			{
				if (IsOn == value)
					return;

				SetValue(_isOn, value);

				epOnButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				lblOnText.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				epOffButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
				lblOffText.Visibility = value ? Visibility.Collapsed : Visibility.Visible;

				OnIsOnStatusChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region Event
		public event EventHandler IsOnStatusChanged;
		#endregion


		#region Constructor
		public SwitchButton()
		{
			this.InitializeComponent();
		} 
		#endregion


		#region Protected Method
		protected void OnIsOnStatusChanged(EventArgs e)
		{
			if (IsOnStatusChanged == null)
				return;

			IsOnStatusChanged(this, e);
		}
		#endregion


		#region Public Method
		public void Toggle()
		{
			this.IsOn = !this.IsOn;
		}
		#endregion


		#region Event Process
		private static void OnIsOnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var obj = o as SwitchButton;
			obj.IsOn = (bool)e.NewValue;
		}

		private void epOffButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void epOnButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void lblOffText_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void lblOnText_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Toggle();
		} 
		#endregion
	}
}