namespace InfiniteStorage
{
	partial class ProgressTooltip
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressTooltip));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tabControlEx1 = new InfiniteStorage.TabControlEx();
			this.tabProgress = new System.Windows.Forms.TabPage();
			this.devname = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.progressText = new System.Windows.Forms.Label();
			this.tabDisconnected = new System.Windows.Forms.TabPage();
			this.devname3 = new System.Windows.Forms.Label();
			this.disconnectedText = new System.Windows.Forms.Label();
			this.disconnect_desc = new System.Windows.Forms.Label();
			this.tabComplete = new System.Windows.Forms.TabPage();
			this.importCompleteText = new System.Windows.Forms.Label();
			this.devname2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabControlEx1.SuspendLayout();
			this.tabProgress.SuspendLayout();
			this.tabDisconnected.SuspendLayout();
			this.tabComplete.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::InfiniteStorage.Properties.Resources.Waiting;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// tabControlEx1
			// 
			resources.ApplyResources(this.tabControlEx1, "tabControlEx1");
			this.tabControlEx1.Controls.Add(this.tabProgress);
			this.tabControlEx1.Controls.Add(this.tabDisconnected);
			this.tabControlEx1.Controls.Add(this.tabComplete);
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 3;
			this.tabControlEx1.SelectedIndex = 0;
			// 
			// tabProgress
			// 
			this.tabProgress.BackColor = System.Drawing.SystemColors.Control;
			this.tabProgress.Controls.Add(this.devname);
			this.tabProgress.Controls.Add(this.progressBar1);
			this.tabProgress.Controls.Add(this.progressText);
			resources.ApplyResources(this.tabProgress, "tabProgress");
			this.tabProgress.Name = "tabProgress";
			// 
			// devname
			// 
			resources.ApplyResources(this.devname, "devname");
			this.devname.Name = "devname";
			// 
			// progressBar1
			// 
			resources.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.Name = "progressBar1";
			// 
			// progressText
			// 
			resources.ApplyResources(this.progressText, "progressText");
			this.progressText.Name = "progressText";
			// 
			// tabDisconnected
			// 
			this.tabDisconnected.BackColor = System.Drawing.SystemColors.Control;
			this.tabDisconnected.Controls.Add(this.devname3);
			this.tabDisconnected.Controls.Add(this.disconnectedText);
			this.tabDisconnected.Controls.Add(this.disconnect_desc);
			resources.ApplyResources(this.tabDisconnected, "tabDisconnected");
			this.tabDisconnected.Name = "tabDisconnected";
			// 
			// devname3
			// 
			resources.ApplyResources(this.devname3, "devname3");
			this.devname3.Name = "devname3";
			// 
			// disconnectedText
			// 
			resources.ApplyResources(this.disconnectedText, "disconnectedText");
			this.disconnectedText.Name = "disconnectedText";
			// 
			// disconnect_desc
			// 
			resources.ApplyResources(this.disconnect_desc, "disconnect_desc");
			this.disconnect_desc.Name = "disconnect_desc";
			// 
			// tabComplete
			// 
			this.tabComplete.BackColor = System.Drawing.SystemColors.Control;
			this.tabComplete.Controls.Add(this.importCompleteText);
			this.tabComplete.Controls.Add(this.devname2);
			resources.ApplyResources(this.tabComplete, "tabComplete");
			this.tabComplete.Name = "tabComplete";
			this.tabComplete.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// importCompleteText
			// 
			resources.ApplyResources(this.importCompleteText, "importCompleteText");
			this.importCompleteText.Name = "importCompleteText";
			this.importCompleteText.Click += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// devname2
			// 
			resources.ApplyResources(this.devname2, "devname2");
			this.devname2.Name = "devname2";
			this.devname2.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			// 
			// ProgressTooltip
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControlEx1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressTooltip";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressTooltip_FormClosing);
			this.Load += new System.EventHandler(this.ProgressTooltip_Load);
			this.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabControlEx1.ResumeLayout(false);
			this.tabProgress.ResumeLayout(false);
			this.tabDisconnected.ResumeLayout(false);
			this.tabComplete.ResumeLayout(false);
			this.tabComplete.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label devname;
		private System.Windows.Forms.Label progressText;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label disconnect_desc;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabProgress;
		private System.Windows.Forms.TabPage tabDisconnected;
		private System.Windows.Forms.Label devname3;
		private System.Windows.Forms.Label disconnectedText;
		private System.Windows.Forms.TabPage tabComplete;
		private System.Windows.Forms.Label importCompleteText;
		private System.Windows.Forms.Label devname2;
	}
}