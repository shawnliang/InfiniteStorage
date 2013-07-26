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


		public bool SyncOldPhotos
		{
			get { return syncOldPhotos.IsChecked == true; }
		}

		public bool SyncAll
		{
			get { return syncAll.IsChecked == true; }
		}

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

		private void SyncNowButton_Click(object sender, RoutedEventArgs e)
		{
			SyncNow = true;
			Close();
		}

		
	}
}
