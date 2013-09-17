#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using Waveface.Common;
using log4net;

#endregion

namespace InfiniteStorage
{
	public enum DebugLevel
	{
		DEBUG,
		INFO,
		WARN,
		ERROR
	}

	public partial class AboutControl : UserControl
	{
		public event EventHandler SettingsChanged;

		public AboutControl()
		{
			InitializeComponent();
		}

		private void AboutControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				versionLabel.Text = string.Format(Resources.VersionLable, Assembly.GetExecutingAssembly().GetName().Version);
				comboBox1.SelectedItem = Settings.Default.LogLevel;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			raiseSettingsChangedEvent();
		}

		private void raiseSettingsChangedEvent()
		{
			var handler = SettingsChanged;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public DebugLevel LogLevel
		{
			get
			{
				if (comboBox1.SelectedItem != null)
				{
					DebugLevel level;

					if (Enum.TryParse(comboBox1.SelectedItem.ToString(), out level))
						return level;
				}

				return DebugLevel.WARN;
			}

			set { comboBox1.SelectedItem = value.ToString(); }
		}

		private void openLogButton_Click(object sender, EventArgs e)
		{
			var file = Path.Combine(MyFileFolder.AppData, "server.log");

			if (File.Exists(file))
				Process.Start(file);
		}

		private void checkForUpdateButton_Click(object sender, EventArgs e)
		{
			try
			{
				var update = new AutoUpdate(false);

				if (update.IsUpdateRequired())
					update.ShowUpdateNeededUI();
				else
					MessageBox.Show(Resources.AlreadyLastestVersion, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogManager.GetLogger(GetType()).Error("Unable to check for update", err);
			}
		}
	}
}