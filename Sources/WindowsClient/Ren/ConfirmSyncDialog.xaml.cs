using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Waveface.Client
{
	/// <summary>
	/// ConfirmSyncDialog.xaml 的互動邏輯
	/// </summary>
	public partial class ConfirmSyncDialog : Window
	{
		public InfiniteStorage.Data.Pairing.pairing_request PairingRequest { get; set; }
		public bool SyncNow { get; private set; }
		public bool SyncAll { get; private set; }

		public ObservableCollection<Uri> Thumbnails
		{
			get;
			private set;
		}

		public ConfirmSyncDialog()
		{
			InitializeComponent();
			Thumbnails = new ObservableCollection<Uri>();
			photoWall.ItemsSource = Thumbnails;
		}

		private void Window_Loaded_1(object sender, RoutedEventArgs e)
		{
			connected_title.Text = string.Format(connected_title.Text.ToString(), PairingRequest.device_name);
		}

		private void ImportLatest150Button_Click(object sender, RoutedEventArgs e)
		{
			SyncNow = true;
			SyncAll = false;
			Close();
		}

		private void ImportAllButton_Click(object sender, RoutedEventArgs e)
		{
			SyncNow = true;
			SyncAll = true;
			Close();
		}
	}
}
