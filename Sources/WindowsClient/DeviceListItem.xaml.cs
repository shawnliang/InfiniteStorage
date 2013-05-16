using System.Windows;
using System.Windows.Controls;

namespace WpfApplication3
{
	/// <summary>
	/// Interaction logic for DeviceListItem.xaml
	/// </summary>
	public partial class DeviceListItem : UserControl
	{
		#region Var
		public static readonly DependencyProperty _deviceName = DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceListItem), new UIPropertyMetadata(default(string), new PropertyChangedCallback(OnDeviceNameChanged)));
		#endregion

		#region Property
		public string DeviceName
		{
			get
			{
				return (string)GetValue(_deviceName);
			}
			set
			{
				SetValue(_deviceName, value);
				this.lblDeviceName.Content = value;
			}
		}
		#endregion


		#region Constructor
		public DeviceListItem()
		{
			InitializeComponent();
		}
		#endregion


		#region Event Process
		private static void OnDeviceNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var control = o as DeviceListItem;
			control.DeviceName = (string)e.NewValue;
		}
		#endregion
	}
}