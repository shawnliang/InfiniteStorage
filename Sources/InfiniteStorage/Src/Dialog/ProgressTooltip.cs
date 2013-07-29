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
		}

		public void UpdateComplete(int importedCount)
		{
			progressText.Text = string.Format(Resources.ProgressTooltip_Complete, importedCount);
			progressBar1.Hide();
			pictureBox1.Image = Resources.check;
			tabControlEx1.SelectedTab = finishedTab;

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
			if (tabControlEx1.SelectedTab == inProgressTab)
			{
				e.Cancel = true;
				Hide();
			}
		}

		private void viewButton_Click(object sender, EventArgs e)
		{
			ImportUIPresenter.Instance.StartViewer(device_id);
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void hideButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
	}
}
