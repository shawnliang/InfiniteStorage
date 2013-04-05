using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using System.ComponentModel;

namespace Gui
{
	[ToolboxItem(false)]
	public partial class WelcomeStep : ModernInfoStep
	{
		InstallationMode mode;
		public WelcomeStep(InstallationMode mode)
		{
			this.mode = mode;
			InitializeComponent();
			lblMode.Text = Gui.Properties.Resources.ResourceManager.GetString("WelcomeStepGreeting" + mode.ToString()) ?? lblMode.Text;
		}

		private void WelcomeStep_OnNext(object sender, ChangeStepEventArgs e)
		{
			Wizard.LifecycleAction(LifecycleActionType.ModeSelected, mode);
		}
	}
}
