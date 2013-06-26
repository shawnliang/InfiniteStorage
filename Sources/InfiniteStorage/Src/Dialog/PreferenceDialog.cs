using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;


namespace InfiniteStorage
{
	public partial class PreferenceDialog : Form
	{
		public PreferenceDialog()
		{
			InitializeComponent();
			Text = Resources.ProductName;
			Icon = Resources.ProductIcon;

			deviceListControl.SettingChanged += handleAnySettingChanged;
			aboutControl1.SettingsChanged += handleAnySettingChanged;
			generalPreferenceControl1.SettingsChanged += handleAnySettingChanged;
			tabAbout.Text = string.Format(Resources.AboutTab, Resources.ProductName);
			homeSharingControl1.SettingsChanged += handleAnySettingChanged;

			generalPreferenceControl1.Enabled = true;
		}

		public PreferenceDialog(StationServer station)
			:this()
		{
			generalPreferenceControl1.Station = station;
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
				Settings.Default.LibraryName = generalPreferenceControl1.LibraryName;
				Settings.Default.Save();

				saveDeviceListChanges();
				setAutoStartValue();
				saveLogLevel();
				saveHomeSharing();
				return true;
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to apply changes", err);
				MessageBox.Show(err.Message, "Unable to save settings", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;
			}
		}

		private void saveHomeSharing()
		{
			bool somethingChanged = false;

			if (homeSharingControl1.HomeSharingEnabled != HomeSharing.Enabled)
			{
				HomeSharing.Enabled = homeSharingControl1.HomeSharingEnabled;
				somethingChanged = true;
			}

			if (homeSharingControl1.PasswordRequired != Settings.Default.HomeSharingPasswordRequired)
			{
				Settings.Default.HomeSharingPasswordRequired = homeSharingControl1.PasswordRequired;
				somethingChanged = true;
			}

			if (homeSharingControl1.Password != Settings.Default.HomeSharingPassword)
			{
				Settings.Default.HomeSharingPassword = homeSharingControl1.Password;
				somethingChanged = true;
			}

			if (somethingChanged)
			{
				Settings.Default.Save();

				if (homeSharingControl1.HomeSharingEnabled)
				{
					NginxUtility.Instance.PrepareNginxConfig(12888, Settings.Default.SingleFolderLocation);
					NginxUtility.Instance.Start();
				}
				else
				{
					NginxUtility.Instance.Stop();
				}

				BonjourServiceRegistrator.Instance.Register();
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

				devConns.ToList().ForEach(x =>
				{
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
			var value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "com.waveface.FavoriteHome", null);

			checkboxAutoRun.Checked = !string.IsNullOrEmpty(value as string);
		}

		private void setAutoStartValue()
		{
			if (checkboxAutoRun.Checked)
			{
				var value = "\"" + Assembly.GetExecutingAssembly().Location + "\" --minimized";
				Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "com.waveface.FavoriteHome", value);
			}
			else
			{
				var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
				key.DeleteValue("com.waveface.FavoriteHome", false);
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}
	}
}
