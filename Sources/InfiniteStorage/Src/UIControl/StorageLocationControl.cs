using InfiniteStorage.Properties;
using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.Win32;
using System.Diagnostics;

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

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var target = Path.Combine(dialog.SelectedPath, Resources.ProductName);

				var confirm = MessageBox.Show(
					string.Format(Resources.MoveFolderConfirmMsg, target),
					Resources.AreYouSure, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

				if (confirm == DialogResult.Cancel)
					return;


				Cursor.Current = Cursors.WaitCursor;
				this.Enabled = false;

				Station.Stop();
				
				var bgworker = new BackgroundWorker();
				bgworker.DoWork += new DoWorkEventHandler(bgworker_DoWork);
				bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgworker_RunWorkerCompleted);
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

		void bgworker_DoWork(object sender, DoWorkEventArgs e)
		{

			closeClientProgram();

			Station.MoveFolder(e.Argument as string);

			e.Result = e.Argument;
		}

		void bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message, Resources.MoveFolderFailTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				log4net.LogManager.GetLogger(GetType()).Warn("Move folder unsuccessful", e.Error);
			}
			else if (e.Cancelled)
			{
				MessageBox.Show("Cancelled");
			}
			else
			{
				storageLocationBox.Text = (string)e.Result;
				Settings.Default.SingleFolderLocation = e.Result as string;
				Settings.Default.Save();
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", e.Result);
			}


			Station.Start();

			Cursor.Current = Cursors.Default;
			this.Enabled = true;		
		}

		

		public string StoragePath
		{
			get { return storageLocationBox.Text; }
			set { storageLocationBox.Text = value; }
		}

		public StationServer Station { get; set; }
	}
}
