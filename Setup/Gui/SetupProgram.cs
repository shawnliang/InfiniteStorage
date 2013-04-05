using System;
using System.Windows.Forms;
using SharpSetup.Base;

namespace Gui
{
	static class SetupProgram
	{
		/// <summary>
		/// The main entry point for the installer.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			SetupHelper.Initialize(args);
			SetupHelper.Install += new EventHandler<EventArgs>(SetupHelper_Install);
			SetupHelper.SilentInstall += new EventHandler<EventArgs>(SetupHelper_SilentInstall);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			SetupHelper.StartInstallation();
		}

		static void SetupHelper_SilentInstall(object sender, EventArgs e)
		{
			MsiConnection.Instance.Open(Gui.Properties.Resources.MainMsiFile, true);
			MsiConnection.Instance.Install();
		}

		static void SetupHelper_Install(object sender, EventArgs e)
		{
			Application.Run(new Gui.SetupWizard());
		}
	}
}
