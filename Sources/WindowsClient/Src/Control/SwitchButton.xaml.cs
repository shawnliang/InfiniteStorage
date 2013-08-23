#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for SwitchButton.xaml
	/// </summary>
	public partial class SwitchButton : UserControl
	{
		#region Var

		public static readonly DependencyProperty _isOn = DependencyProperty.Register("IsOn", typeof(bool), typeof(SwitchButton), new UIPropertyMetadata(false, OnIsOnChanged));

		#endregion

		#region Property

		public string OffText
		{
			set { lblOffText.Content = value; }
			get { return lblOffText.Content.ToString(); }
		}

		public string OnText
		{
			set { lblOnText.Content = value; }
			get { return lblOnText.Content.ToString(); }
		}

		public bool IsOn
		{
			get { return (bool)GetValue(_isOn); }
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
			InitializeComponent();
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
			IsOn = !IsOn;
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

		private void epOffButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void epOnButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void lblOffText_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Toggle();
		}

		private void lblOnText_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Toggle();
		}

		#endregion
	}
}