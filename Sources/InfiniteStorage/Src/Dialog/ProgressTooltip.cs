using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class ProgressTooltip : Form
	{
		private static ProgressTooltip instance = new ProgressTooltip();

		private string currDeviceId;

		public static ProgressTooltip Instance
		{
			get { return instance; }
		}

		public ProgressTooltip()
		{
			InitializeComponent();
		}

		public void OnFileEnding(object sender, WebsocketEventArgs args)
		{
			var file_id = args.ctx.fileCtx.file_id;
			var file_name = args.ctx.fileCtx.file_name;
			var file_type = args.ctx.fileCtx.type;

			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				showFile(file_id, file_name, file_type, args.ctx.device_name, args.ctx.device_id);
			});
		}

		private void showFile(Guid file_id, string file_name, FileAssetType type, string dev, string dev_id)
		{
			try
			{
				this.filename.Text = file_name;
				this.Text = string.Format(Resources.ProgressTooltip, dev);
				this.currDeviceId = dev_id;

				PendingFile file = null;
				using (var db = new MyDbContext())
				{
					var q = from f in db.Object.PendingFiles
							where f.file_id == file_id
							select f;


					file = q.FirstOrDefault();
				}

				if (file == null)
					return;

				if (type == FileAssetType.image)
				{
					pictureBox1.ImageLocation = Path.Combine(MyFileFolder.Photo, ".pending", file.saved_path);
				}
				else
				{
					pictureBox1.Image = Resources.video;
				}

				Show();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn(err.ToString());
			}
		}

		public void ShowWaitingDevice(string dev)
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(() => { showWaiting(dev); }));
			}
			else
			{
				showWaiting(dev);
			}
		}

		private void showWaiting(string dev)
		{
			pictureBox1.Image = Resources.Waiting;
			filename.Text = dev + Resources.MenuItem_Preparing;
			Show();
		}

		private void ProgressTooltip_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;

			this.StartPosition = FormStartPosition.Manual;
			Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width,
								 Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height);
		}

		private void ProgressTooltip_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}

		private void filename_DoubleClick(object sender, EventArgs e)
		{
			ImportUIPresenter.Instance.StartViewer();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ImportUIPresenter.Instance.StartViewer();
		}
	}
}
