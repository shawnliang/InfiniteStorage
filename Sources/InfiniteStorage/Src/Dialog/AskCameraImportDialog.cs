#region

using System;
using System.Windows.Forms;
using InfiniteStorage.Properties;

#endregion

namespace InfiniteStorage
{
	public partial class AskCameraImportDialog : Form
	{
		public string DeviceName
		{
			set { devName.Text = value; }
		}

		private string m_deviceId { get; set; }

		public AskCameraImportDialog()
		{
			InitializeComponent();

			Icon = Resources.ProductIcon;
		}

		public AskCameraImportDialog(string deviceName, string deviceId)
		{
			InitializeComponent();

			DeviceName = deviceName;
			m_deviceId = deviceId;
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
					Settings.Default.AlwaysImportDevices.Add(m_deviceId);
				else
					Settings.Default.AlwaysNotImportDevices.Add(m_deviceId);

				Settings.Default.Save();
			}

			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;

			Close();
		}
	}
}