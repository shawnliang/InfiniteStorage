using System;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;
using System.IO;
using System.Reflection;
using System.Diagnostics;

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
					closeRunningProcess();

					MsiConnection.Instance.Uninstall();

					if (File.Exists(Resources.MainMsiFile))
						MsiConnection.Instance.Open(Resources.MainMsiFile, true);
				}
				else if (mode == InstallationMode.Install)
				{
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

		private static void closeRunningProcess()
		{
			try
			{
				var procs = Process.GetProcessesByName("InfiniteStorage");
				foreach (var proc in procs)
				{
					proc.Kill();
				}
			}
			catch
			{
			}
		}

		public override bool CanClose()
		{
			return false;
		}
	}
}
