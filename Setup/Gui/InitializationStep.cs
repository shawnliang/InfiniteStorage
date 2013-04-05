using System;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using System.IO;
using System.Reflection;

namespace Gui
{
	[System.ComponentModel.ToolboxItem(false)]
	public partial class InitializationStep : ModernInfoStep
	{
		public InitializationStep()
		{
			InitializeComponent();
		}

		private void InitializationStep_Entered(object sender, EventArgs e)
		{
			Application.DoEvents();
			if (File.Exists(Gui.Properties.Resources.MainMsiFile))
				MsiConnection.Instance.Open(Gui.Properties.Resources.MainMsiFile, true);
			else
				MsiConnection.Instance.Open(SetupHelper.GetProductGuidFromPath());
			Wizard.LifecycleAction(LifecycleActionType.ConnectionOpened);
			Wizard.NextStep();
		}
	}
}
