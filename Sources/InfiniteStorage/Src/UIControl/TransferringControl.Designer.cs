namespace InfiniteStorage
{
	partial class TransferringControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.progressLabel = new System.Windows.Forms.Label();
			this.fileLable = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.label3 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// progressLabel
			// 
			this.progressLabel.AutoSize = true;
			this.progressLabel.Location = new System.Drawing.Point(272, 169);
			this.progressLabel.Name = "progressLabel";
			this.progressLabel.Size = new System.Drawing.Size(106, 13);
			this.progressLabel.TabIndex = 9;
			this.progressLabel.Text = "進度: 準備傳送中...";
			// 
			// fileLable
			// 
			this.fileLable.AutoSize = true;
			this.fileLable.Location = new System.Drawing.Point(272, 189);
			this.fileLable.Name = "fileLable";
			this.fileLable.Size = new System.Drawing.Size(106, 13);
			this.fileLable.TabIndex = 8;
			this.fileLable.Text = "檔案: 準備傳送中...";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::InfiniteStorage.Properties.Resources.Tea_Cup1;
			this.pictureBox1.Location = new System.Drawing.Point(28, 120);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(216, 124);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 7;
			this.pictureBox1.TabStop = false;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(275, 208);
			this.progressBar.Maximum = 2000;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(217, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(25, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(219, 34);
			this.label3.TabIndex = 5;
			this.label3.Text = "第一次備份比較花時間, 先去泡杯茶享受一下吧~";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// TransferringControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.progressLabel);
			this.Controls.Add(this.fileLable);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.label3);
			this.Name = "TransferringControl";
			this.Size = new System.Drawing.Size(530, 295);
			this.Load += new System.EventHandler(this.TransferringControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.Label fileLable;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer timer1;
	}
}
