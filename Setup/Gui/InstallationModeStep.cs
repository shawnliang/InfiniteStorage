using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using System.Collections.ObjectModel;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InstallationModeStep : ModernInfoStep
	{
		public InstallationModeStep(InstallationModeCollection installationModes)
		{
			InitializeComponent();
			imsModes.Modes.Clear();
			imsModes.Modes.AddRange(installationModes);
		}

		private void InstallationModeStep_OnNext(object sender, ChangeStepEventArgs e)
		{
			Wizard.LifecycleAction(LifecycleActionType.ModeSelected, imsModes.SelectedMode);
		}
	}
}
