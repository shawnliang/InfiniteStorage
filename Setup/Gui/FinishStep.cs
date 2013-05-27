using System;
using System.Diagnostics;
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using Microsoft.Win32;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class FinishStep : ModernInfoStep
	{
		InstallationMode mode;

		public FinishStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
		}

		private void FinishStep_Entered(object sender, EventArgs e)
		{
			Wizard.BackButton.Enabled = false;

			cbRunNow.Checked = cbRunNow.Visible = (mode == InstallationMode.Install || mode == InstallationMode.Upgrade);
		}

		private void FinishStep_Finish(object sender, ChangeStepEventArgs e)
		{
			if (cbRunNow.Checked && cbRunNow.Visible)
			{				
				var program = string.Format(Gui.Properties.Resources.FinishStepCommand, MsiConnection.Instance.GetPath("INSTALLLOCATION"));
				UACHelper.CreateProcessAsStandardUser(program, "");
			}

			if (mode == InstallationMode.Install)
			{
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", Wizard.GetVariable<string>("resourceFolder"));
			}
		}

		private void FinishStep_Entering(object sender, ChangeStepEventArgs e)
		{
			cbRunNow.Visible = Wizard.GetVariable<bool>("AllowRunOnFinish");
		}
	}
}
