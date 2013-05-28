namespace Gui
{
	partial class InstallationModeStep
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationModeStep));
			this.lblInstruction = new System.Windows.Forms.Label();
			this.lblDetails = new System.Windows.Forms.Label();
			this.lblWelcome = new System.Windows.Forms.Label();
			this.imsModes = new SharpSetup.UI.Controls.InstallationModeSelector();
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
			this.SuspendLayout();
			// 
			// pbLeft
			// 
			resources.ApplyResources(this.pbLeft, "pbLeft");
			// 
			// lblInstruction
			// 
			resources.ApplyResources(this.lblInstruction, "lblInstruction");
			this.lblInstruction.Name = "lblInstruction";
			// 
			// lblDetails
			// 
			resources.ApplyResources(this.lblDetails, "lblDetails");
			this.lblDetails.Name = "lblDetails";
			// 
			// lblWelcome
			// 
			resources.ApplyResources(this.lblWelcome, "lblWelcome");
			this.lblWelcome.Name = "lblWelcome";
			// 
			// imsModes
			// 
			resources.ApplyResources(this.imsModes, "imsModes");
			this.imsModes.Modes.Add(SharpSetup.Base.InstallationMode.Upgrade);
			this.imsModes.Modes.Add(SharpSetup.Base.InstallationMode.Uninstall);
			this.imsModes.Name = "imsModes";
			this.imsModes.SelectedMode = SharpSetup.Base.InstallationMode.None;
			// 
			// InstallationModeStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.imsModes);
			this.Controls.Add(this.lblInstruction);
			this.Controls.Add(this.lblDetails);
			this.Controls.Add(this.lblWelcome);
			this.Name = "InstallationModeStep";
			this.MoveNext += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.InstallationModeStep_OnNext);
			this.Controls.SetChildIndex(this.pbLeft, 0);
			this.Controls.SetChildIndex(this.lblWelcome, 0);
			this.Controls.SetChildIndex(this.lblDetails, 0);
			this.Controls.SetChildIndex(this.lblInstruction, 0);
			this.Controls.SetChildIndex(this.imsModes, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblInstruction;
		private System.Windows.Forms.Label lblDetails;
		private System.Windows.Forms.Label lblWelcome;
		private SharpSetup.UI.Controls.InstallationModeSelector imsModes;
	}
}
