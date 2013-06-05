namespace Gui
{
	partial class WelcomeStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeStep));
			this.lblWelcome = new System.Windows.Forms.Label();
			this.lblMode = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblInstruction = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
			this.SuspendLayout();
			// 
			// pbLeft
			// 
			resources.ApplyResources(this.pbLeft, "pbLeft");
			// 
			// lblWelcome
			// 
			resources.ApplyResources(this.lblWelcome, "lblWelcome");
			this.lblWelcome.Name = "lblWelcome";
			// 
			// lblMode
			// 
			resources.ApplyResources(this.lblMode, "lblMode");
			this.lblMode.Name = "lblMode";
			// 
			// lblCopyright
			// 
			resources.ApplyResources(this.lblCopyright, "lblCopyright");
			this.lblCopyright.Name = "lblCopyright";
			// 
			// lblInstruction
			// 
			resources.ApplyResources(this.lblInstruction, "lblInstruction");
			this.lblInstruction.Name = "lblInstruction";
			// 
			// WelcomeStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstruction);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.lblMode);
			this.Controls.Add(this.lblWelcome);
			this.Name = "WelcomeStep";
			this.MoveNext += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.WelcomeStep_OnNext);
			this.Controls.SetChildIndex(this.pbLeft, 0);
			this.Controls.SetChildIndex(this.lblWelcome, 0);
			this.Controls.SetChildIndex(this.lblMode, 0);
			this.Controls.SetChildIndex(this.lblCopyright, 0);
			this.Controls.SetChildIndex(this.lblInstruction, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Label lblMode;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Label lblInstruction;
	}
}
