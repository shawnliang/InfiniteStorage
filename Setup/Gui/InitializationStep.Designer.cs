namespace Gui
{
	partial class InitializationStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitializationStep));
			this.lblInstall = new System.Windows.Forms.Label();
			this.lblInitializing = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
			this.SuspendLayout();
			// 
			// lblInstall
			// 
			resources.ApplyResources(this.lblInstall, "lblInstall");
			this.lblInstall.Name = "lblInstall";
			// 
			// lblInitializing
			// 
			resources.ApplyResources(this.lblInitializing, "lblInitializing");
			this.lblInitializing.Name = "lblInitializing";
			// 
			// InitializationStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstall);
			this.Controls.Add(this.lblInitializing);
			this.Name = "InitializationStep";
			this.StepType = SharpSetup.UI.Forms.Modern.ModernStepType.TransitionaryOnetime;
			this.Entered += new System.EventHandler<System.EventArgs>(this.InitializationStep_Entered);
			this.Controls.SetChildIndex(this.pbLeft, 0);
			this.Controls.SetChildIndex(this.lblInitializing, 0);
			this.Controls.SetChildIndex(this.lblInstall, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblInstall;
		private System.Windows.Forms.Label lblInitializing;

	}
}
