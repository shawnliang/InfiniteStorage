using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using System.IO;
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;
using Gui.Properties;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InstallationLocationStep1 : ModernActionStep
	{
		public InstallationLocationStep1()
		{
			InitializeComponent();
		}

		private void InstallationLocationStep1_Entering(object sender, ChangeStepEventArgs e)
		{
			location.Text = Path.Combine(Environment.GetEnvironmentVariable("UserProfile"), "Piary Photos");
			
			if (!isLocationValid())
				errorProvider1.SetError(location, Resources.LocationStep_NotEmpty);

			try
			{
				var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				Directory.Delete(Path.Combine(localAppData, "waveface"), true);
			}
			catch
			{
			}
			
			try
			{
				var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				Directory.Delete(Path.Combine(appData, "Bunny"), true);
			}
			catch
			{
			}
		}

		private void changeButton_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Path.GetDirectoryName(location.Text);


			if (dialog.ShowDialog() == DialogResult.OK)
				location.Text = Path.Combine(dialog.SelectedPath, "Piary Photos");
		}

		private void InstallationLocationStep1_MoveNext(object sender, ChangeStepEventArgs e)
		{
			Wizard.SetVariable<string>("resourceFolder", location.Text);

			if (!isLocationValid())
			{
				e.Continue = false;
				MessageBox.Show(Resources.LocationStep_NotEmpty);
			}
		}

		private void location_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (isLocationValid())
				errorProvider1.SetError(location, "");
			else
				errorProvider1.SetError(location, Resources.LocationStep_NotEmpty);
		}

		private bool isLocationValid()
		{
			var dir = new DirectoryInfo(location.Text);
			// location must be empty or not exist
			return !dir.Exists || !dir.GetFileSystemInfos().Any();
		}
	}
}
