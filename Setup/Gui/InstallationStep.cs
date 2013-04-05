using System;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;
using System.IO;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InstallationStep : ModernActionStep
	{
		InstallationMode mode;
		public InstallationStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
		}

		private void InstallationStep_Entered(object sender, EventArgs e)
		{
			ipProgress.StartListening();
			try
			{
				if (mode == InstallationMode.Uninstall)
				{
					MsiConnection.Instance.Uninstall();
					/*
					try
					{
						MsiConnection.Instance.Open(new Guid("{74a76f6a-ba58-4a07-828f-a0d3a0e937b3}"));
						MsiConnection.Instance.Uninstall();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Uninstall");
					}
					*/
					if (File.Exists(Resources.MainMsiFile))
						MsiConnection.Instance.Open(Resources.MainMsiFile, true);
				}
				else if (mode == InstallationMode.Install)
				{
					/*
					MsiConnection.Instance.SaveAs("MainInstall");
					MsiConnection.Instance.Open("other.msi", false);
					MsiConnection.Instance.Install("");
					MsiConnection.Instance.OpenSaved("MainInstall");
					*/
					MsiConnection.Instance.Install();
				}
				else
					MessageBox.Show("Unknown mode");
			}
			catch (MsiException mex)
			{
				if (mex.ErrorCode != (uint)InstallError.UserExit)
					MessageBox.Show("Installation failed: " + mex.Message);
				Wizard.Finish();
			}
			ipProgress.StopListening();
			Wizard.NextStep();
		}

		public override bool CanClose()
		{
			return false;
		}
	}
}
