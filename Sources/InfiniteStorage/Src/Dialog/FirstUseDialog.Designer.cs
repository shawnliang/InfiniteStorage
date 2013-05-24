namespace InfiniteStorage
{
	partial class FirstUseDialog
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.nextButton = new System.Windows.Forms.Button();
			this.prevButton = new System.Windows.Forms.Button();
			this.tabControlEx1 = new InfiniteStorage.TabControlEx();
			this.tabWelcomeAndWaitConnect = new System.Windows.Forms.TabPage();
			this.startPairingUserControl1 = new InfiniteStorage.StartPairingUserControl();
			this.tabChooseLocation = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.storageLocationControl1 = new InfiniteStorage.StorageLocationControl();
			this.tabInstalledSuccess = new System.Windows.Forms.TabPage();
			this.transferringControl1 = new InfiniteStorage.TransferringControl();
			this.panel1.SuspendLayout();
			this.tabControlEx1.SuspendLayout();
			this.tabWelcomeAndWaitConnect.SuspendLayout();
			this.tabChooseLocation.SuspendLayout();
			this.tabInstalledSuccess.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.nextButton);
			this.panel1.Controls.Add(this.prevButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 323);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(628, 46);
			this.panel1.TabIndex = 0;
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nextButton.Location = new System.Drawing.Point(541, 6);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(75, 32);
			this.nextButton.TabIndex = 1;
			this.nextButton.Text = "下一步";
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// prevButton
			// 
			this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.prevButton.Location = new System.Drawing.Point(12, 6);
			this.prevButton.Name = "prevButton";
			this.prevButton.Size = new System.Drawing.Size(75, 32);
			this.prevButton.TabIndex = 0;
			this.prevButton.Text = "上一步";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// tabControlEx1
			// 
			this.tabControlEx1.Controls.Add(this.tabWelcomeAndWaitConnect);
			this.tabControlEx1.Controls.Add(this.tabChooseLocation);
			this.tabControlEx1.Controls.Add(this.tabInstalledSuccess);
			this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 1;
			this.tabControlEx1.SelectedIndex = 0;
			this.tabControlEx1.Size = new System.Drawing.Size(628, 323);
			this.tabControlEx1.TabIndex = 1;
			this.tabControlEx1.SelectedIndexChanged += new System.EventHandler(this.tabControlEx1_SelectedIndexChanged);
			// 
			// tabWelcomeAndWaitConnect
			// 
			this.tabWelcomeAndWaitConnect.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabWelcomeAndWaitConnect.Controls.Add(this.startPairingUserControl1);
			this.tabWelcomeAndWaitConnect.Location = new System.Drawing.Point(0, 0);
			this.tabWelcomeAndWaitConnect.Name = "tabWelcomeAndWaitConnect";
			this.tabWelcomeAndWaitConnect.Padding = new System.Windows.Forms.Padding(3);
			this.tabWelcomeAndWaitConnect.Size = new System.Drawing.Size(628, 323);
			this.tabWelcomeAndWaitConnect.TabIndex = 0;
			this.tabWelcomeAndWaitConnect.Text = "歡迎使用 Bunny";
			// 
			// startPairingUserControl1
			// 
			this.startPairingUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.startPairingUserControl1.Location = new System.Drawing.Point(3, 3);
			this.startPairingUserControl1.Name = "startPairingUserControl1";
			this.startPairingUserControl1.Size = new System.Drawing.Size(622, 317);
			this.startPairingUserControl1.TabIndex = 0;
			// 
			// tabChooseLocation
			// 
			this.tabChooseLocation.Controls.Add(this.label4);
			this.tabChooseLocation.Controls.Add(this.label5);
			this.tabChooseLocation.Controls.Add(this.storageLocationControl1);
			this.tabChooseLocation.Location = new System.Drawing.Point(0, 0);
			this.tabChooseLocation.Name = "tabChooseLocation";
			this.tabChooseLocation.Padding = new System.Windows.Forms.Padding(3);
			this.tabChooseLocation.Size = new System.Drawing.Size(628, 323);
			this.tabChooseLocation.TabIndex = 3;
			this.tabChooseLocation.Text = "選擇備份資料夾位置";
			this.tabChooseLocation.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(68, 77);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(492, 2);
			this.label4.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label5.Location = new System.Drawing.Point(12, 23);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(604, 45);
			this.label5.TabIndex = 2;
			this.label5.Text = "再也不怕手機照片、影片、錄音檔遺失或手機空間不夠了\r\n只要電腦開著，檔案就會自己跑回來！";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// storageLocationControl1
			// 
			this.storageLocationControl1.Location = new System.Drawing.Point(68, 114);
			this.storageLocationControl1.Name = "storageLocationControl1";
			this.storageLocationControl1.Size = new System.Drawing.Size(492, 95);
			this.storageLocationControl1.TabIndex = 0;
			// 
			// tabInstalledSuccess
			// 
			this.tabInstalledSuccess.Controls.Add(this.transferringControl1);
			this.tabInstalledSuccess.Location = new System.Drawing.Point(0, 0);
			this.tabInstalledSuccess.Name = "tabInstalledSuccess";
			this.tabInstalledSuccess.Padding = new System.Windows.Forms.Padding(3);
			this.tabInstalledSuccess.Size = new System.Drawing.Size(628, 323);
			this.tabInstalledSuccess.TabIndex = 2;
			this.tabInstalledSuccess.Text = "設定完成";
			this.tabInstalledSuccess.UseVisualStyleBackColor = true;
			// 
			// transferringControl1
			// 
			this.transferringControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.transferringControl1.Location = new System.Drawing.Point(3, 3);
			this.transferringControl1.Name = "transferringControl1";
			this.transferringControl1.Size = new System.Drawing.Size(622, 317);
			this.transferringControl1.TabIndex = 0;
			this.transferringControl1.WebSocketContext = null;
			// 
			// FirstUseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(628, 369);
			this.Controls.Add(this.tabControlEx1);
			this.Controls.Add(this.panel1);
			this.Name = "FirstUseDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "[product name]";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirstUseDialog_FormClosing);
			this.Load += new System.EventHandler(this.FirstUseDialog_Load);
			this.panel1.ResumeLayout(false);
			this.tabControlEx1.ResumeLayout(false);
			this.tabWelcomeAndWaitConnect.ResumeLayout(false);
			this.tabChooseLocation.ResumeLayout(false);
			this.tabInstalledSuccess.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabWelcomeAndWaitConnect;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.TabPage tabInstalledSuccess;
		private System.Windows.Forms.TabPage tabChooseLocation;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private StorageLocationControl storageLocationControl1;
		private TransferringControl transferringControl1;
		private StartPairingUserControl startPairingUserControl1;
	}
}