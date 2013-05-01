using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class TransferringControl : UserControl
	{
		public ProtocolContext WebSocketContext { get; set; }


		public TransferringControl()
		{
			InitializeComponent();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{

				if (WebSocketContext != null)
				{

					if (!WebSocketContext.IsClosed)
					{
						progressLabel.Text = string.Format(Resources.FirstUse_ProgressLabel, WebSocketContext.recved_files, WebSocketContext.total_files);


						// TODO: accessing member properties of WebScokectContext without locking
						// may cause race condition. Best practice is to use a critical section to 
						// ensure the consistency of fileCtx and temp_file. But due to this status 
						// is transient, catching exception to recover from transient fault is also
						// "OK" right now.


						var fileInfo = WebSocketContext.fileCtx;
						if (fileInfo != null)
						{
							fileLable.Text = string.Format(Resources.FirstUse_FileLabel, fileInfo.file_name, fileInfo.file_size);
							progressBar.Style = ProgressBarStyle.Blocks;
							progressBar.Minimum = 0;
							progressBar.Maximum = (int)fileInfo.file_size;
							progressBar.Value = WebSocketContext.temp_file.BytesWritten;
						}
					}
					else
					{
						progressLabel.Text = string.Format(Resources.FirstUse_TransferStopped, WebSocketContext.device_name, WebSocketContext.recved_files);
						progressBar.Visible = fileLable.Visible = false;
					}
				}
			}
			catch (Exception error)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to update backup progress", error);
			}
		}

		private void TransferringControl_Load(object sender, EventArgs e)
		{
			timer1.Start();
		}

		public void StopUpdateUI()
		{
			timer1.Stop();
		}
	}
}
