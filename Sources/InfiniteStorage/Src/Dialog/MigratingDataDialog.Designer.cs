namespace InfiniteStorage
{
	partial class MigratingDataDialog
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 75);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(359, 23);
			this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(359, 28);
			this.label1.TabIndex = 1;
			this.label1.Text = "Piary Photos is checking data... This takes a few seconds. Please wait.";
			// 
			// MigratingDataDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(383, 124);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.progressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MigratingDataDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Checking Data...";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MigratingDataDialog_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
	}
}