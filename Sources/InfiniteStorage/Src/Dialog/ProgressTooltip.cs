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
		private string device_id;
		public bool UserHide { get; set; }

		public ProgressTooltip()
		{
			InitializeComponent();
			this.progressText.Text = Resources.MenuItem_Preparing;
		}

		public ProgressTooltip(string deviceName, string deviceId)
			: this()
		{
			this.devname.Text = deviceName;
			this.device_id = deviceId;
		}

		private void ProgressTooltip_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;
			tabControlEx1.SelectedTab = inProgressTab;

			this.StartPosition = FormStartPosition.Manual;
			Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width,
								 Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height);
		}

		public void UpdateProgress(int current, int totoal, int percentage)
		{
			progressText.Text = string.Format(Resources.ProgressTooltip_Progress, current, totoal, percentage);
			progressBar1.Value = percentage;
			progressBar1.Visible = true;
			tabControlEx1.SelectedTab = inProgressTab;

			if (!UserHide)
			{
				Show();
			}
		}

		public void UpdateComplete(int importedCount)
		{
			progressText.Text = string.Format(Resources.ProgressTooltip_Complete, importedCount);
			progressBar1.Hide();
			pictureBox1.ImageLocation = null;
			pictureBox1.Image = Resources.check;
			tabControlEx1.SelectedTab = finishedTab;

			UserHide = false;
			Show();
		}

		public void UpdateInterrupted(int received_count)
		{
			progressText.Text = string.Format(Resources.ProgressTooltip_Interrupted, received_count);
			progressBar1.Hide();
			tabControlEx1.SelectedTab = finishedTab;

			UserHide = false;
			Show();
		}

		public void UpdateImage(string file_path)
		{
			pictureBox1.ImageLocation = file_path;
		}

		public void UpdateImageToVideoIcon()
		{
			pictureBox1.Image = Resources.video;
		}

		private void ProgressTooltip_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;

				if (tabControlEx1.SelectedTab == inProgressTab)
				{
					UserHide = true;
				}
				else
				{
					// keep UserHide == false, so that new photo from existing ws connection
					// will trigger progress tooltip
				}

				Hide();
			}
		}

		private void viewButton_Click(object sender, EventArgs e)
		{
			ImportUIPresenter.Instance.StartViewer(device_id);
			// keep UserHide == false, so that new photo from existing ws connection
			// will trigger progress tooltip
			Hide();
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			// keep UserHide == false, so that new photo from existing ws connection
			// will trigger progress tooltip
			Hide();
		}

		private void hideButton_Click(object sender, EventArgs e)
		{
			UserHide = true;
			Hide();
		}
	}

}
