using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class DeviceListControl : UserControl
	{
		public List<Device> DeletedDevices { get; private set; }
		public event EventHandler SettingChanged;

		private BindingSource dataSource;

		public DeviceListControl()
		{
			InitializeComponent();
			DeletedDevices = new List<Device>();
		}

		public void RefreshData()
		{
			if (!bgPopulate.IsBusy)
			{
				bgPopulate.RunWorkerAsync();
			}
		}
		private void DeviceListControl_Load(object sender, EventArgs e)
		{
			dataGridView1.AutoGenerateColumns = false;

			if (!DesignMode)
			{
				bgPopulate.RunWorkerAsync();

				radioAllow.Checked = !Settings.Default.RejectOtherDevices;
				radioDisallow.Checked = Settings.Default.RejectOtherDevices;

				showBackupProgress.Checked = Settings.Default.ShowBackupProgressDialog;
			}
		}

		private List<Device> populateData()
		{
			using (var db = new MyDbContext())
			{
				return db.Object.Devices.ToList();
			}
		}

		private void bgPopulate_DoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = populateData();
		}

		private void bgPopulate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Error populate device list", e.Error);
				MessageBox.Show(e.Error.Message, "Unable to populate device list", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (e.Result != null)
			{
				dataGridView1.DataSource = dataSource = new BindingSource(e.Result as List<Device>, null);
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex != dataGridView1.Columns["delCol"].Index)
				return;

			if (MessageBox.Show(Resources.DeleteDevice_Description, Resources.DeleteDevice_Title, MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				var device = dataGridView1.Rows[e.RowIndex].DataBoundItem as Device;

				DeletedDevices.Add(device);
				dataSource.Remove(device);

				raiseSettingChangedEvent();
			}
		}

		public bool RejectOtherDevices
		{
			get { return radioDisallow.Checked; }
			set { radioDisallow.Checked = value; }
		}

		public bool ShowBackupProgress
		{
			get { return showBackupProgress.Checked; }
			set { showBackupProgress.Checked = value; }
		}

		private void raiseSettingChangedEvent()
		{
			var handler = SettingChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		private void rejectOtherDevices_CheckedChanged(object sender, EventArgs e)
		{
			raiseSettingChangedEvent();
		}

		private void showBackupProgress_Click(object sender, EventArgs e)
		{
			raiseSettingChangedEvent();
		}
	}
}
