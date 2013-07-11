using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using readCamera;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class AskCameraImportDialog : Form
	{
		public string DeviceName
		{
			set { devName.Text = value; }
		}

		public AskCameraImportDialog()
		{
			InitializeComponent();
			Icon = Resources.ProductIcon;
		}

		public AskCameraImportDialog(string deviceName)
		{
			InitializeComponent();
			this.DeviceName = deviceName;
		}

		private void AskCameraImportDialog_Load(object sender, EventArgs e)
		{
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
