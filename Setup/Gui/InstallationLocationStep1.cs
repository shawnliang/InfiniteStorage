using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using System.IO;
using System;
using System.Windows.Forms;

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
			//if (!Wizard.GetVariable<bool>("CustomInstallation"))
			//    Wizard.ContinueMove();


			location.Text = Path.Combine(Environment.GetEnvironmentVariable("UserProfile"), "FavoriteHome");
		}

		private void changeButton_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.SelectedPath = location.Text;
			if (dialog.ShowDialog() == DialogResult.OK)
				location.Text = dialog.SelectedPath;
		}

		private void InstallationLocationStep1_MoveNext(object sender, ChangeStepEventArgs e)
		{
			Wizard.SetVariable<string>("resourceFolder", location.Text);
		}
	}
}
