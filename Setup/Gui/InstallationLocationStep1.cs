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
			location.Text = Path.Combine(Environment.GetEnvironmentVariable("UserProfile"), "Favorite Home");
		}

		private void changeButton_Click(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Path.GetDirectoryName(location.Text);


			if (dialog.ShowDialog() == DialogResult.OK)
				location.Text = Path.Combine(dialog.SelectedPath, "Favorite Home");
		}

		private void InstallationLocationStep1_MoveNext(object sender, ChangeStepEventArgs e)
		{
			Wizard.SetVariable<string>("resourceFolder", location.Text);
		}
	}
}
