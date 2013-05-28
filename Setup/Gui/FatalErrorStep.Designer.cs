namespace Gui
{
	partial class FatalErrorStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FatalErrorStep));
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblDetails = new System.Windows.Forms.Label();
			this.lblInstruction = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
			this.SuspendLayout();
			// 
			// pbLeft
			// 
			resources.ApplyResources(this.pbLeft, "pbLeft");
			// 
			// lblTitle
			// 
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.Name = "lblTitle";
			// 
			// lblDetails
			// 
			resources.ApplyResources(this.lblDetails, "lblDetails");
			this.lblDetails.Name = "lblDetails";
			// 
			// lblInstruction
			// 
			resources.ApplyResources(this.lblInstruction, "lblInstruction");
			this.lblInstruction.Name = "lblInstruction";
			// 
			// FatalErrorStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstruction);
			this.Controls.Add(this.lblDetails);
			this.Controls.Add(this.lblTitle);
			this.Name = "FatalErrorStep";
			this.StepType = SharpSetup.UI.Forms.Modern.ModernStepType.Last;
			this.Controls.SetChildIndex(this.pbLeft, 0);
			this.Controls.SetChildIndex(this.lblTitle, 0);
			this.Controls.SetChildIndex(this.lblDetails, 0);
			this.Controls.SetChildIndex(this.lblInstruction, 0);
			((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblDetails;
		private System.Windows.Forms.Label lblInstruction;
	}
}
