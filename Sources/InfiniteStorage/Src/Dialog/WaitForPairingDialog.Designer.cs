namespace InfiniteStorage
{
	partial class WaitForPairingDialog
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
			this.startPairingUserControl1 = new InfiniteStorage.StartPairingUserControl();
			this.SuspendLayout();
			// 
			// startPairingUserControl1
			// 
			this.startPairingUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.startPairingUserControl1.Location = new System.Drawing.Point(0, 0);
			this.startPairingUserControl1.Name = "startPairingUserControl1";
			this.startPairingUserControl1.Size = new System.Drawing.Size(623, 336);
			this.startPairingUserControl1.TabIndex = 0;
			// 
			// WaitForPairingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(623, 336);
			this.Controls.Add(this.startPairingUserControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WaitForPairingDialog";
			this.Text = "WaitForPairingDialog";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WaitForPairingDialog_FormClosed);
			this.Load += new System.EventHandler(this.WaitForPairingDialog_Load);
			this.Shown += new System.EventHandler(this.WaitForPairingDialog_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private StartPairingUserControl startPairingUserControl1;

	}
}