namespace InfiniteStorage
{
	partial class PairingRequestDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PairingRequestDialog));
			this.yesButton = new System.Windows.Forms.Button();
			this.neverButton = new System.Windows.Forms.Button();
			this.noButton = new System.Windows.Forms.Button();
			this.questionLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// yesButton
			// 
			resources.ApplyResources(this.yesButton, "yesButton");
			this.yesButton.Name = "yesButton";
			this.yesButton.UseVisualStyleBackColor = true;
			this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
			// 
			// neverButton
			// 
			resources.ApplyResources(this.neverButton, "neverButton");
			this.neverButton.Name = "neverButton";
			this.neverButton.UseVisualStyleBackColor = true;
			this.neverButton.Click += new System.EventHandler(this.neverButton_Click);
			// 
			// noButton
			// 
			resources.ApplyResources(this.noButton, "noButton");
			this.noButton.Name = "noButton";
			this.noButton.UseVisualStyleBackColor = true;
			this.noButton.Click += new System.EventHandler(this.noButton_Click);
			// 
			// questionLabel
			// 
			resources.ApplyResources(this.questionLabel, "questionLabel");
			this.questionLabel.Name = "questionLabel";
			// 
			// PairingRequestDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.questionLabel);
			this.Controls.Add(this.noButton);
			this.Controls.Add(this.neverButton);
			this.Controls.Add(this.yesButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "PairingRequestDialog";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PairingRequestDialog_FormClosing);
			this.Load += new System.EventHandler(this.PairingRequestDialog_Load);
			this.Shown += new System.EventHandler(this.PairingRequestDialog_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button yesButton;
		private System.Windows.Forms.Button neverButton;
		private System.Windows.Forms.Button noButton;
		private System.Windows.Forms.Label questionLabel;
	}
}