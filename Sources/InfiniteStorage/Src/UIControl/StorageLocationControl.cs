using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class StorageLocationControl : UserControl
	{
		public StorageLocationControl()
		{
			InitializeComponent();			
		}

		private void StorageLocationControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				var storageLocation = Settings.Default.SingleFolderLocation;
				if (string.IsNullOrEmpty(storageLocation) || Settings.Default.IsFirstUse)
					storageLocation = Path.Combine(MediaLibrary.UserFolder, Resources.ProductName);

				storageLocationBox.Text = storageLocation;
			}
		}

		private void changeStorageButton_Click(object sender, EventArgs e)
		{
			var openLocation = Path.GetDirectoryName(storageLocationBox.Text);
			var dialog = new FolderBrowserDialog
			{
				SelectedPath = openLocation
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				storageLocationBox.Text = Path.Combine(dialog.SelectedPath, Resources.ProductName);
			}
		}

		public string StoragePath
		{
			get { return storageLocationBox.Text; }
			set { storageLocationBox.Text = value; }
		}
	}
}
