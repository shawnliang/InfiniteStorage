namespace Gui
{
	partial class InstallationStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationStep));
			this.ipProgress = new SharpSetup.UI.Controls.InstallationProgress();
			this.pbInstallation = new System.Windows.Forms.PictureBox();
			this.lblInstallation = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbInstallation)).BeginInit();
			this.SuspendLayout();
			// 
			// ipProgress
			// 
			resources.ApplyResources(this.ipProgress, "ipProgress");
			this.ipProgress.Name = "ipProgress";
			// 
			// pbInstallation
			// 
			resources.ApplyResources(this.pbInstallation, "pbInstallation");
			this.pbInstallation.Image = global::Gui.Properties.Resources.InstallationStepImg;
			this.pbInstallation.Name = "pbInstallation";
			this.pbInstallation.TabStop = false;
			// 
			// lblInstallation
			// 
			resources.ApplyResources(this.lblInstallation, "lblInstallation");
			this.lblInstallation.Name = "lblInstallation";
			// 
			// InstallationStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstallation);
			this.Controls.Add(this.pbInstallation);
			this.Controls.Add(this.ipProgress);
			this.Name = "InstallationStep";
			this.StepType = SharpSetup.UI.Forms.Modern.ModernStepType.TransitionaryOnetime;
			this.Entered += new System.EventHandler<System.EventArgs>(this.InstallationStep_Entered);
			this.Controls.SetChildIndex(this.ipProgress, 0);
			this.Controls.SetChildIndex(this.pbInstallation, 0);
			this.Controls.SetChildIndex(this.lblInstallation, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbInstallation)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SharpSetup.UI.Controls.InstallationProgress ipProgress;
		private System.Windows.Forms.PictureBox pbInstallation;
		private System.Windows.Forms.Label lblInstallation;
	}
}
