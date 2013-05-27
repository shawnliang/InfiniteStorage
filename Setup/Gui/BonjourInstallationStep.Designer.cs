namespace Gui
{
	partial class BonjourInstallationStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BonjourInstallationStep));
			this.pbInstallation = new System.Windows.Forms.PictureBox();
			this.lblInstallation = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.progressLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbInstallation)).BeginInit();
			this.SuspendLayout();
			// 
			// pbInstallation
			// 
			resources.ApplyResources(this.pbInstallation, "pbInstallation");
			this.pbInstallation.Image = global::Gui.Properties.Resources.BonjourInstallationStepImg;
			this.pbInstallation.Name = "pbInstallation";
			this.pbInstallation.TabStop = false;
			// 
			// lblInstallation
			// 
			resources.ApplyResources(this.lblInstallation, "lblInstallation");
			this.lblInstallation.Name = "lblInstallation";
			// 
			// progressBar1
			// 
			resources.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			// 
			// progressLabel
			// 
			resources.ApplyResources(this.progressLabel, "progressLabel");
			this.progressLabel.Name = "progressLabel";
			// 
			// BonjourInstallationStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstallation);
			this.Controls.Add(this.pbInstallation);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.progressLabel);
			this.Name = "BonjourInstallationStep";
			this.StepType = SharpSetup.UI.Forms.Modern.ModernStepType.TransitionaryOnetime;
			this.Entered += new System.EventHandler<System.EventArgs>(this.BonjourInstallationStep_Entered);
			this.Controls.SetChildIndex(this.progressLabel, 0);
			this.Controls.SetChildIndex(this.progressBar1, 0);
			this.Controls.SetChildIndex(this.pbInstallation, 0);
			this.Controls.SetChildIndex(this.lblInstallation, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbInstallation)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pbInstallation;
		private System.Windows.Forms.Label lblInstallation;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label progressLabel;
	}
}
