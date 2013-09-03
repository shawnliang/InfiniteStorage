﻿using InfiniteStorage.Properties;
using System;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class AskCameraImportDialog : Form
	{
		public string DeviceName
		{
			set { devName.Text = value; }
		}

		private string deviceId { get; set; }

		public AskCameraImportDialog()
		{
			InitializeComponent();
			Icon = Resources.ProductIcon;
		}

		public AskCameraImportDialog(string deviceName, string deviceId)
		{
			InitializeComponent();
			this.DeviceName = deviceName;
			this.deviceId = deviceId;
		}

		private void AskCameraImportDialog_Load(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = radioImport.Checked ? DialogResult.OK : DialogResult.Cancel;

			if (checkBoxRemember.Checked)
			{
				if (radioImport.Checked)
					Settings.Default.AlwaysImportDevices.Add(deviceId);
				else
					Settings.Default.AlwaysNotImportDevices.Add(deviceId);

				Settings.Default.Save();
			}

			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
