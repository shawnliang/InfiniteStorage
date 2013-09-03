using InfiniteStorage.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class ProgressTooltip : Form
	{
		private string device_id;

		private bool inCompleteState;
		public bool UserHide { get; set; }

		public ProgressTooltip()
		{
			InitializeComponent();
			this.progressText.Text = Resources.MenuItem_Preparing;
		}

		public ProgressTooltip(string deviceName, string deviceId)
			: this()
		{
			this.devname.Text = this.devname2.Text = this.devname3.Text = deviceName;
			this.device_id = deviceId;
		}

		private void ProgressTooltip_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;

			this.StartPosition = FormStartPosition.Manual;
			Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width,
								 Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height);
		}

		public void UpdateProgress(int current, int totoal, int percentage)
		{
			tabControlEx1.SelectedTab = tabProgress;
			progressText.Text = string.Format(Resources.ProgressTooltip_Progress, current, totoal, percentage);
			progressBar1.Value = percentage;

			if (!UserHide)
			{
				Show();
			}
		}

		public void UpdateComplete(int importedCount, int total)
		{
			tabControlEx1.SelectedTab = tabComplete;
			importCompleteText.Text = string.Format(Resources.ProgressTooltip_Complete, importedCount, total);
			pictureBox1.ImageLocation = null;
			pictureBox1.Image = Resources.check;

			inCompleteState = true;
			UserHide = false;
			Show();
		}

		public void UpdateInterrupted(int received, int total)
		{
			tabControlEx1.SelectedTab = tabDisconnected;
			disconnectedText.Text = string.Format(Resources.ProgressTooltip_Interrupted, received, total);

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

				if (inCompleteState)
				{
					// keep UserHide == false, so that new photo from existing ws connection
					// will trigger progress tooltip
				}
				else
				{
					UserHide = true;
				}

				Hide();
			}
		}

		private void viewButton_Click(object sender, EventArgs e)
		{
			MainUIWrapper.Instance.StartViewer(device_id);
			// keep UserHide == false, so that new photo from existing ws connection
			// will trigger progress tooltip
			Hide();
		}

		private void onDoubleClick(object sender, EventArgs e)
		{
			MainUIWrapper.Instance.StartViewer(device_id);
		}
	}

}
