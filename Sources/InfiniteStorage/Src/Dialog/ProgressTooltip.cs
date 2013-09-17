#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InfiniteStorage.Properties;

#endregion

namespace InfiniteStorage
{
	public partial class ProgressTooltip : Form
	{
		private string device_id;
		private bool inCompleteState;

		private static List<ProgressTooltip> dialogs = new List<ProgressTooltip>();

		public bool UserHide { get; set; }

		private ProgressTooltip()
		{
			InitializeComponent();

			progressText.Text = Resources.MenuItem_Preparing;
		}

		public ProgressTooltip(string deviceName, string deviceId)
			: this()
		{
			devname.Text = devname2.Text = devname3.Text = deviceName;
			device_id = deviceId;
		}

		private void ProgressTooltip_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;

			StartPosition = FormStartPosition.Manual;
			Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Size.Width,
			                     Screen.PrimaryScreen.WorkingArea.Height - Size.Height);
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

		/*
		private void viewButton_Click(object sender, EventArgs e)
		{
			MainUIWrapper.Instance.StartViewer(device_id);
			// keep UserHide == false, so that new photo from existing ws connection
			// will trigger progress tooltip
			Hide();
		}
		*/

		private void onDoubleClick(object sender, EventArgs e)
		{
			MainUIWrapper.Instance.StartViewer(device_id);
		}

		private void ProgressTooltip_Shown(object sender, EventArgs e)
		{
			dialogs.Add(this);
		}

		private void ProgressTooltip_FormClosed(object sender, FormClosedEventArgs e)
		{
			dialogs.Remove(this);
		}

		public static void RemoveDialog(string deviceId)
		{
			var toRemove = (from d in dialogs
			                where d.device_id == deviceId
			                select d).ToList();

			foreach (var del in toRemove)
			{
				del.Close();
				dialogs.Remove(del);
			}
		}
	}
}