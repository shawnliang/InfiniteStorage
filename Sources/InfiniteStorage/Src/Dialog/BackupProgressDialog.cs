using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using System;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class BackupProgressDialog : Form
	{
		public ProtocolContext WSCtx { get; set; }

		public BackupProgressDialog()
		{
			InitializeComponent();
		}

		public BackupProgressDialog(ProtocolContext ctx)
			: this()
		{
			this.WSCtx = ctx;
		}

		private void BackupProgressDialog_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;
			TitleLabel.Text = string.Format(Resources.BackupProgressTitle_InProgress, WSCtx.device_name);
			refresh_timer.Start();
		}

		private void refresh_timer_Tick(object sender, EventArgs e)
		{
			try
			{
				if (WSCtx.IsClosed || WSCtx.NoMoreToTransfer())
				{
					TitleLabel.Text = string.Format(Resources.BackupProgressTitle_Finished, WSCtx.device_name);
					overallLabel.Text = (WSCtx.NoMoreToTransfer()) ?
						string.Format(Resources.BackupProgressTitle_BackupComplete, WSCtx.total_count) :
						string.Format(Resources.FirstUse_TransferStopped, WSCtx.device_name, WSCtx.recved_files);

					curFileLabel.Text = "";
					progressBar.Visible = false;
				}
				else
				{
					TitleLabel.Text = string.Format(Resources.BackupProgressTitle_InProgress, WSCtx.device_name);
					overallLabel.Text = string.Format(Resources.FirstUse_ProgressLabel, WSCtx.backup_count, WSCtx.total_count);
					if (WSCtx.fileCtx != null)
					{
						curFileLabel.Text = string.Format(Resources.FirstUse_FileLabel, WSCtx.fileCtx.file_name, WSCtx.fileCtx.file_size);

						int curVal, maxVal;
						Normalizer.NormalizeToInt(WSCtx.fileCtx.file_size, WSCtx.temp_file.BytesWritten, out maxVal, out curVal);

						progressBar.Maximum = maxVal;
						progressBar.Value = curVal;
						progressBar.Visible = true;
					}
					else
					{
						curFileLabel.Text = Resources.BackupProgressFile_Preparing;
						progressBar.Value = 0;
						progressBar.Visible = true;
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to update backup progress", err);
			}
		}


		private void BackupProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			refresh_timer.Stop();
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (dontBotherMe.Checked)
			{
				Settings.Default.ShowBackupProgressDialog = false;
				Settings.Default.Save();
			}

			Close();
		}
	}
}
