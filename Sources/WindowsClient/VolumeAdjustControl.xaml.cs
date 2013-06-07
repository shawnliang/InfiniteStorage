using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for VolumeAdjustControl.xaml
	/// </summary>
	public partial class VolumeAdjustControl : UserControl 
	{
		public double Volume
		{ 
			get
			{
				return VolumeProgress.Value;
			}
			set
			{
				VolumeProgress.Value = value;
			}
		}


		#region Event
		public event EventHandler VolumeChanged;
		#endregion

		public VolumeAdjustControl()
		{
			this.InitializeComponent();
		}

		protected void OnVolumeChanged(EventArgs e)
		{
			if (VolumeChanged == null)
				return;
			VolumeChanged(this, e);
		}

		private void VolumeProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			OnVolumeChanged(EventArgs.Empty);
		}
	}
}