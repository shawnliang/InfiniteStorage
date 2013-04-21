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
			this.yesButton = new System.Windows.Forms.Button();
			this.neverButton = new System.Windows.Forms.Button();
			this.noButton = new System.Windows.Forms.Button();
			this.questionLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// yesButton
			// 
			this.yesButton.Location = new System.Drawing.Point(12, 36);
			this.yesButton.Name = "yesButton";
			this.yesButton.Size = new System.Drawing.Size(75, 23);
			this.yesButton.TabIndex = 0;
			this.yesButton.Text = "是";
			this.yesButton.UseVisualStyleBackColor = true;
			this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
			// 
			// neverButton
			// 
			this.neverButton.Location = new System.Drawing.Point(183, 36);
			this.neverButton.Name = "neverButton";
			this.neverButton.Size = new System.Drawing.Size(143, 23);
			this.neverButton.TabIndex = 1;
			this.neverButton.Text = "拒絕所有連線邀請";
			this.neverButton.UseVisualStyleBackColor = true;
			this.neverButton.Click += new System.EventHandler(this.neverButton_Click);
			// 
			// noButton
			// 
			this.noButton.Location = new System.Drawing.Point(93, 36);
			this.noButton.Name = "noButton";
			this.noButton.Size = new System.Drawing.Size(75, 23);
			this.noButton.TabIndex = 2;
			this.noButton.Text = "否";
			this.noButton.UseVisualStyleBackColor = true;
			this.noButton.Click += new System.EventHandler(this.noButton_Click);
			// 
			// questionLabel
			// 
			this.questionLabel.AutoSize = true;
			this.questionLabel.Location = new System.Drawing.Point(12, 9);
			this.questionLabel.Name = "questionLabel";
			this.questionLabel.Size = new System.Drawing.Size(55, 13);
			this.questionLabel.TabIndex = 3;
			this.questionLabel.Text = "[Question]";
			// 
			// PairingRequestDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(338, 71);
			this.Controls.Add(this.questionLabel);
			this.Controls.Add(this.noButton);
			this.Controls.Add(this.neverButton);
			this.Controls.Add(this.yesButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "PairingRequestDialog";
			this.Text = "PairingRequestDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PairingRequestDialog_FormClosing);
			this.Load += new System.EventHandler(this.PairingRequestDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button yesButton;
		private System.Windows.Forms.Button neverButton;
		private System.Windows.Forms.Button noButton;
		private System.Windows.Forms.Label questionLabel;
	}
}