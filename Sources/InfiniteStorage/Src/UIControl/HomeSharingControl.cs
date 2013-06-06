using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using Microsoft.Win32;

namespace InfiniteStorage.Src.UIControl
{
	public partial class HomeSharingControl : UserControl
	{
		public event EventHandler SettingsChanged;

		public HomeSharingControl()
		{
			InitializeComponent();

		}

		private void HomeSharingControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				var enabled = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "HomeSharing", "true");

				enableHomeSharing.Checked = enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase);
				pwdProtection.Checked = Settings.Default.HomeSharingPasswordRequired;
				password.Text = Settings.Default.HomeSharingPassword;
			}

			updateUI();
		}

		private void enableHomeSharing_CheckedChanged(object sender, EventArgs e)
		{
			updateUI();
			raiseSettingsChanged();
		}

		private void showPassword_CheckedChanged(object sender, EventArgs e)
		{
			password.PasswordChar = (showPassword.Checked) ? '\0' : '*';
		}

		private void pwdProtection_CheckedChanged(object sender, EventArgs e)
		{
			updateUI();
			raiseSettingsChanged();
		}

		private void password_TextChanged(object sender, EventArgs e)
		{
			updateUI();
			raiseSettingsChanged();
		}

		private void updateUI()
		{
			lblHomeSharingExplain.Enabled = pwdProtection.Enabled = showPassword.Enabled = password.Enabled = enableHomeSharing.Checked;

			showPassword.Enabled = password.Enabled = pwdProtection.Checked && pwdProtection.Enabled;

			invalidPwd.Visible = password.Enabled && password.TextLength != 4;
		}

		private void password_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\b')
				return;

			e.Handled = e.KeyChar < '0' || e.KeyChar > '9' || password.Text.Length >= 4;
		}

		private void raiseSettingsChanged()
		{
			var handler = SettingsChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public bool HomeSharingEnabled
		{
			get { return enableHomeSharing.Checked; }
		}

		public string Password
		{
			get { return password.Text; }
		}

		public bool PasswordRequired
		{
			get { return pwdProtection.Checked; }
		}
	}
}
