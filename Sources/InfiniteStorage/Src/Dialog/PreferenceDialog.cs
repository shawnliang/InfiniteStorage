using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using InfiniteStorage.Model;
using Microsoft.Win32;
using System.Reflection;


namespace InfiniteStorage
{
	public partial class PreferenceDialog : Form
	{
		public PreferenceDialog()
		{
			InitializeComponent();
			Text = Resources.ProductName;
			Icon = Resources.product_icon;

			deviceListControl.SettingChanged += new EventHandler(handleAnySettingChanged);
			aboutControl1.SettingsChanged += new EventHandler(handleAnySettingChanged);
			tabAbout.Text = string.Format(Resources.AboutTab, Resources.ProductName);
		}

		void handleAnySettingChanged(object sender, EventArgs e)
		{
			buttonApply.Enabled = true;
		}

		private void PreferenceDialog_Load(object sender, EventArgs e)
		{
			loadAutoStartValue();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (saveChanges())
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			if (saveChanges())
				buttonApply.Enabled = false;
		}

		private bool saveChanges()
		{
			try
			{
				saveDeviceListChanges();
				setAutoStartValue();
				saveLogLevel();
				return true;
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to apply changes", err);
				MessageBox.Show(err.Message, "Unable to save settings", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private void saveLogLevel()
		{
			try
			{
				Settings.Default.LogLevel = aboutControl1.LogLevel.ToString();
				Settings.Default.Save();

				Log4netConfigure.SetLevel(aboutControl1.LogLevel);
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to set debug log level: " + aboutControl1.LogLevel.ToString(), e);
			}
		}

		private void saveDeviceListChanges()
		{
			Settings.Default.RejectOtherDevices = deviceListControl.RejectOtherDevices;
			Settings.Default.ShowBackupProgressDialog = deviceListControl.ShowBackupProgress;
			Settings.Default.Save();

			if (!deviceListControl.DeletedDevices.Any())
				return;

			using (var db = new MyDbContext())
			{
				
				foreach (var dev in deviceListControl.DeletedDevices)
				{
					db.Object.Database.ExecuteSqlCommand("delete from Devices where device_id=?", dev.device_id);
				}
			}

			var conns = ConnectedClientCollection.Instance.GetAllConnections();

			foreach (var dev in deviceListControl.DeletedDevices)
			{
				var devConns = conns.Where((x) => x.device_id == dev.device_id);

				devConns.ToList().ForEach(x => {
					try
					{
						x.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "Removed by user");
					}
					catch (Exception err)
					{
						log4net.LogManager.GetLogger(GetType()).Warn("Unable to send stop ws cmd to " + x.device_name, err);
					}
				});
			}
		}

		private void loadAutoStartValue()
		{
			var value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "com.waveface.infiniteStorage", null);

			checkboxAutoRun.Checked = !string.IsNullOrEmpty(value as string);
		}

		private void setAutoStartValue()
		{
			if (checkboxAutoRun.Checked)
			{
				var value = "\"" + Assembly.GetExecutingAssembly().Location + "\"";
				Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "com.waveface.infiniteStorage", value);
			}
			else
			{
				var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
				key.DeleteValue("com.waveface.infiniteStorage", false);
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}
	}
}
