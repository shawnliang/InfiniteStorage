using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Model;

namespace InfiniteStorage
{
	public partial class DeviceListControl : UserControl
	{
		public DeviceListControl()
		{
			InitializeComponent();
			delCol.UseColumnTextForButtonValue = true;
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
				dataGridView1.DataSource = e.Result;
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex != dataGridView1.Columns["delCol"].Index)
				return;

			var device = dataGridView1.Rows[e.RowIndex].DataBoundItem as Device;

			using (var db = new MyDbContext())
			{
				db.Object.Database.ExecuteSqlCommand("delete from Devices where device_id=?", device.device_id);
			}

			bgPopulate.RunWorkerAsync();
		}

		
	}
}
