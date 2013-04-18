using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Model;
using InfiniteStorage.Properties;

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
				rejectOtherDevices.Checked = Settings.Default.RejectOtherDevices;
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

			var device = dataGridView1.Rows[e.RowIndex].DataBoundItem as Device;

			DeletedDevices.Add(device);
			dataSource.Remove(device);

			raiseSettingChangedEvent();
		}

		public bool RejectOtherDevices
		{
			get { return rejectOtherDevices.Checked; }
			set { rejectOtherDevices.Checked = value; }
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
	}
}
