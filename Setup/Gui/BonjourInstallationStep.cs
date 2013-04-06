using System;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceProcess;
using System.Linq;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class BonjourInstallationStep : ModernActionStep
	{
		InstallationMode mode;
		BackgroundWorker bgworker;

		public BonjourInstallationStep(InstallationMode mode)
		{
			InitializeComponent();
			this.mode = mode;
			this.bgworker = new BackgroundWorker();
		}

		private void BonjourInstallationStep_Entered(object sender, EventArgs e)
		{
			try
			{
				if (mode == InstallationMode.Install)
				{
					if (doesBonjourServiceExist())
					{
						progressLabel.Text = Resources.ConfigureBonjourService;
						//TODO: make sure bonjour service is started and will auto start
						Wizard.NextStep();
					}
					else
					{
						progressLabel.Text = Resources.InstallBonjourService;
						MsiConnection.Instance.SaveAs("com.waveface.infiniteStorage");

						bgworker.DoWork += bgworker_installBonjourService;
						bgworker.RunWorkerCompleted += bgworker_installBonjourServiceCompleted;
						bgworker.RunWorkerAsync();
					}
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
		}

		void bgworker_installBonjourService(object sender, DoWorkEventArgs e)
		{
			var bonjourMsi = is64BitOS() ? "bonjour64.msi" : "bonjour.msi";

			bonjourMsi = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), bonjourMsi);

			var pc = Process.Start(new ProcessStartInfo
			{
				FileName = bonjourMsi
			});

			pc.WaitForExit();

			if (pc.ExitCode != 0)
				e.Cancel = true;
		}

		void bgworker_installBonjourServiceCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			MsiConnection.Instance.OpenSaved("com.waveface.infiniteStorage");

			if (e.Error != null)
			{
				MessageBox.Show(e.Error.ToString());
				throw e.Error;
			}

			if (e.Cancelled)
			{
				Wizard.Finish();
				return;
			}

			Wizard.NextStep();
		}

		

		private bool doesBonjourServiceExist()
		{
			var services = ServiceController.GetServices();
			var bonjourSvc = services.FirstOrDefault(x => x.ServiceName.Equals("Bonjour Service", StringComparison.InvariantCultureIgnoreCase));

			return bonjourSvc != null;
		}

		private bool is64BitOS()
		{
			return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
		}

		public override bool CanClose()
		{
			return false;
		}
	}
}
