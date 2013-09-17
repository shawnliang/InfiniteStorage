#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using Microsoft.Win32;
using log4net;

#endregion

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

				if (string.IsNullOrEmpty(storageLocation))
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

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var target = Path.Combine(dialog.SelectedPath, Resources.ProductName);

				var confirm = MessageBox.Show(
					string.Format(Resources.MoveFolderConfirmMsg, target),
					Resources.AreYouSure, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

				if (confirm == DialogResult.Cancel)
					return;

				Cursor.Current = Cursors.WaitCursor;
				Enabled = false;

				Station.Stop();
				NginxUtility.Instance.Stop();
				Thread.Sleep(3000);

				var bgworker = new BackgroundWorker();
				bgworker.DoWork += bgworker_DoWork;
				bgworker.RunWorkerCompleted += bgworker_RunWorkerCompleted;
				bgworker.RunWorkerAsync(target);
			}
		}

		private void closeClientProgram()
		{
			var clients = Process.GetProcessesByName("Waveface.Client");

			foreach (var client in clients)
			{
				client.CloseMainWindow();
				client.WaitForExit(500);
			}
		}

		private void bgworker_DoWork(object sender, DoWorkEventArgs e)
		{
			closeClientProgram();

			Station.MoveFolder(e.Argument as string);
			e.Result = e.Argument;
		}

		private void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message, Resources.MoveFolderFailTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				LogManager.GetLogger(GetType()).Warn("Move folder unsuccessful", e.Error);
			}
			else if (e.Cancelled)
			{
				MessageBox.Show("Cancelled");
			}
			else
			{
				storageLocationBox.Text = (string) e.Result;
				Settings.Default.SingleFolderLocation = e.Result as string;
				Settings.Default.Save();
				Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", e.Result);
			}

			Station.Start();
			NginxUtility.Instance.PrepareNginxConfig(12888, Settings.Default.SingleFolderLocation);
			NginxUtility.Instance.Start();

			Cursor.Current = Cursors.Default;
			Enabled = true;
		}

		public string StoragePath
		{
			get { return storageLocationBox.Text; }
			set { storageLocationBox.Text = value; }
		}

		public StationServer Station { get; set; }
	}
}