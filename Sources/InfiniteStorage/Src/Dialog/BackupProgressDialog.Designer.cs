namespace InfiniteStorage
{
	partial class BackupProgressDialog
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
			this.components = new System.ComponentModel.Container();
			this.TitleLabel = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.overallLabel = new System.Windows.Forms.Label();
			this.curFileLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.dontBotherMe = new System.Windows.Forms.CheckBox();
			this.refresh_timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// TitleLabel
			// 
			this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TitleLabel.Location = new System.Drawing.Point(13, 13);
			this.TitleLabel.Name = "TitleLabel";
			this.TitleLabel.Size = new System.Drawing.Size(400, 49);
			this.TitleLabel.TabIndex = 0;
			this.TitleLabel.Text = "自 shawn\'s GT-I9300 備份 2000 個檔案自 shawn\'s GT-I9300 備份 ";
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(16, 104);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(397, 15);
			this.progressBar.TabIndex = 1;
			// 
			// overallLabel
			// 
			this.overallLabel.AutoSize = true;
			this.overallLabel.Location = new System.Drawing.Point(13, 62);
			this.overallLabel.Name = "overallLabel";
			this.overallLabel.Size = new System.Drawing.Size(88, 13);
			this.overallLabel.TabIndex = 2;
			this.overallLabel.Text = "進度：準備中...";
			// 
			// curFileLabel
			// 
			this.curFileLabel.AutoSize = true;
			this.curFileLabel.Location = new System.Drawing.Point(13, 83);
			this.curFileLabel.Name = "curFileLabel";
			this.curFileLabel.Size = new System.Drawing.Size(88, 13);
			this.curFileLabel.TabIndex = 3;
			this.curFileLabel.Text = "檔案：準備中...";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(338, 133);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// dontBotherMe
			// 
			this.dontBotherMe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.dontBotherMe.AutoSize = true;
			this.dontBotherMe.Location = new System.Drawing.Point(17, 137);
			this.dontBotherMe.Name = "dontBotherMe";
			this.dontBotherMe.Size = new System.Drawing.Size(110, 17);
			this.dontBotherMe.TabIndex = 5;
			this.dontBotherMe.Text = "再也不要通知我";
			this.dontBotherMe.UseVisualStyleBackColor = true;
			// 
			// refresh_timer
			// 
			this.refresh_timer.Tick += new System.EventHandler(this.refresh_timer_Tick);
			// 
			// BackupProgressDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 168);
			this.Controls.Add(this.dontBotherMe);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.curFileLabel);
			this.Controls.Add(this.overallLabel);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.TitleLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "BackupProgressDialog";
			this.Text = "BackupProgressDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackupProgressDialog_FormClosing);
			this.Load += new System.EventHandler(this.BackupProgressDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label TitleLabel;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label overallLabel;
		private System.Windows.Forms.Label curFileLabel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.CheckBox dontBotherMe;
		private System.Windows.Forms.Timer refresh_timer;
	}
}