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
			this.devname = new System.Windows.Forms.Label();
			this.progressText = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tabControlEx1 = new InfiniteStorage.TabControlEx();
			this.inProgressTab = new System.Windows.Forms.TabPage();
			this.hideButton = new System.Windows.Forms.Button();
			this.finishedTab = new System.Windows.Forms.TabPage();
			this.closeButton = new System.Windows.Forms.Button();
			this.viewButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabControlEx1.SuspendLayout();
			this.inProgressTab.SuspendLayout();
			this.finishedTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Image = global::InfiniteStorage.Properties.Resources.Waiting;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// devname
			// 
			resources.ApplyResources(this.devname, "devname");
			this.devname.Name = "devname";
			// 
			// progressText
			// 
			resources.ApplyResources(this.progressText, "progressText");
			this.progressText.Name = "progressText";
			// 
			// progressBar1
			// 
			resources.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.Name = "progressBar1";
			// 
			// tabControlEx1
			// 
			resources.ApplyResources(this.tabControlEx1, "tabControlEx1");
			this.tabControlEx1.Controls.Add(this.inProgressTab);
			this.tabControlEx1.Controls.Add(this.finishedTab);
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 2;
			this.tabControlEx1.SelectedIndex = 0;
			// 
			// inProgressTab
			// 
			this.inProgressTab.BackColor = System.Drawing.SystemColors.Control;
			this.inProgressTab.Controls.Add(this.hideButton);
			resources.ApplyResources(this.inProgressTab, "inProgressTab");
			this.inProgressTab.Name = "inProgressTab";
			// 
			// hideButton
			// 
			resources.ApplyResources(this.hideButton, "hideButton");
			this.hideButton.Name = "hideButton";
			this.hideButton.UseVisualStyleBackColor = true;
			this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
			// 
			// finishedTab
			// 
			this.finishedTab.BackColor = System.Drawing.SystemColors.Control;
			this.finishedTab.Controls.Add(this.closeButton);
			this.finishedTab.Controls.Add(this.viewButton);
			resources.ApplyResources(this.finishedTab, "finishedTab");
			this.finishedTab.Name = "finishedTab";
			// 
			// closeButton
			// 
			resources.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.Name = "closeButton";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// viewButton
			// 
			resources.ApplyResources(this.viewButton, "viewButton");
			this.viewButton.Name = "viewButton";
			this.viewButton.UseVisualStyleBackColor = true;
			this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
			// 
			// ProgressTooltip
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.tabControlEx1);
			this.Controls.Add(this.progressText);
			this.Controls.Add(this.devname);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressTooltip";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressTooltip_FormClosing);
			this.Load += new System.EventHandler(this.ProgressTooltip_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabControlEx1.ResumeLayout(false);
			this.inProgressTab.ResumeLayout(false);
			this.finishedTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label devname;
		private System.Windows.Forms.Label progressText;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage inProgressTab;
		private System.Windows.Forms.Button hideButton;
		private System.Windows.Forms.TabPage finishedTab;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button viewButton;
		private System.Windows.Forms.ProgressBar progressBar1;
	}
}