using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using System.Reflection;
using System.IO;
using System.Diagnostics;

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
				versionLabel.Text = string.Format(Resources.VersionLable, Assembly.GetExecutingAssembly().GetName().Version.ToString());
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
					if (Enum.TryParse<DebugLevel>(comboBox1.SelectedItem.ToString(), out level))
						return level;
				}

				return DebugLevel.WARN;
			}

			set
			{
				comboBox1.SelectedItem = value.ToString();
			}
		}

		private void openLogButton_Click(object sender, EventArgs e)
		{
			var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName, "server.log");

			if (File.Exists(file))
				Process.Start(file);

		}

		private void checkForUpdateButton_Click(object sender, EventArgs e)
		{
			try
			{
				var update = new Waveface.Common.AutoUpdate(false);

				if (update.IsUpdateRequired())
					update.ShowUpdateNeededUI();
				else
					MessageBox.Show(Resources.AlreadyLastestVersion, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				log4net.LogManager.GetLogger(GetType()).Error("Unable to check for update", err);
			}
		}
	}
}
