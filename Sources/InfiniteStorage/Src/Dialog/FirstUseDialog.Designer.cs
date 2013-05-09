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
			this.pictureBox4 = new System.Windows.Forms.PictureBox();
			this.instruction1 = new System.Windows.Forms.Label();
			this.hrLine = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabChooseLocation = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.storageLocationControl1 = new InfiniteStorage.StorageLocationControl();
			this.tabChooseOrganizeMethod = new System.Windows.Forms.TabPage();
			this.organizeSelectionControl1 = new InfiniteStorage.OrganizeSelectionControl();
			this.label8 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabInstalledSuccess = new System.Windows.Forms.TabPage();
			this.transferringControl1 = new InfiniteStorage.TransferringControl();
			this.panel1.SuspendLayout();
			this.tabControlEx1.SuspendLayout();
			this.tabWelcomeAndWaitConnect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
			this.tabChooseLocation.SuspendLayout();
			this.tabChooseOrganizeMethod.SuspendLayout();
			this.tabInstalledSuccess.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.nextButton);
			this.panel1.Controls.Add(this.prevButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 288);
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
			this.tabControlEx1.Controls.Add(this.tabChooseOrganizeMethod);
			this.tabControlEx1.Controls.Add(this.tabInstalledSuccess);
			this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 3;
			this.tabControlEx1.SelectedIndex = 0;
			this.tabControlEx1.Size = new System.Drawing.Size(628, 288);
			this.tabControlEx1.TabIndex = 1;
			this.tabControlEx1.SelectedIndexChanged += new System.EventHandler(this.tabControlEx1_SelectedIndexChanged);
			// 
			// tabWelcomeAndWaitConnect
			// 
			this.tabWelcomeAndWaitConnect.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabWelcomeAndWaitConnect.Controls.Add(this.pictureBox4);
			this.tabWelcomeAndWaitConnect.Controls.Add(this.instruction1);
			this.tabWelcomeAndWaitConnect.Controls.Add(this.hrLine);
			this.tabWelcomeAndWaitConnect.Controls.Add(this.label1);
			this.tabWelcomeAndWaitConnect.Location = new System.Drawing.Point(0, 0);
			this.tabWelcomeAndWaitConnect.Name = "tabWelcomeAndWaitConnect";
			this.tabWelcomeAndWaitConnect.Padding = new System.Windows.Forms.Padding(3);
			this.tabWelcomeAndWaitConnect.Size = new System.Drawing.Size(628, 288);
			this.tabWelcomeAndWaitConnect.TabIndex = 0;
			this.tabWelcomeAndWaitConnect.Text = "歡迎使用 Bunny";
			// 
			// pictureBox4
			// 
			this.pictureBox4.Image = global::InfiniteStorage.Properties.Resources.ChooseThisPCOnYourPhone;
			this.pictureBox4.Location = new System.Drawing.Point(24, 107);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(297, 200);
			this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox4.TabIndex = 3;
			this.pictureBox4.TabStop = false;
			// 
			// instruction1
			// 
			this.instruction1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.instruction1.Location = new System.Drawing.Point(327, 158);
			this.instruction1.Name = "instruction1";
			this.instruction1.Size = new System.Drawing.Size(281, 77);
			this.instruction1.TabIndex = 2;
			this.instruction1.Text = "請到手機上選擇要備份到這台PC: shawnliangmbp";
			// 
			// hrLine
			// 
			this.hrLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hrLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.hrLine.Location = new System.Drawing.Point(68, 77);
			this.hrLine.Name = "hrLine";
			this.hrLine.Size = new System.Drawing.Size(484, 2);
			this.hrLine.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label1.Location = new System.Drawing.Point(12, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(596, 45);
			this.label1.TabIndex = 0;
			this.label1.Text = "再也不怕手機照片、影片、錄音檔遺失或手機空間不夠了\r\n只要電腦開著，檔案就會自己跑回來！";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabChooseLocation
			// 
			this.tabChooseLocation.Controls.Add(this.label4);
			this.tabChooseLocation.Controls.Add(this.label5);
			this.tabChooseLocation.Controls.Add(this.storageLocationControl1);
			this.tabChooseLocation.Location = new System.Drawing.Point(0, 0);
			this.tabChooseLocation.Name = "tabChooseLocation";
			this.tabChooseLocation.Padding = new System.Windows.Forms.Padding(3);
			this.tabChooseLocation.Size = new System.Drawing.Size(628, 288);
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
			this.storageLocationControl1.StoragePath = "";
			this.storageLocationControl1.TabIndex = 0;
			// 
			// tabChooseOrganizeMethod
			// 
			this.tabChooseOrganizeMethod.Controls.Add(this.organizeSelectionControl1);
			this.tabChooseOrganizeMethod.Controls.Add(this.label8);
			this.tabChooseOrganizeMethod.Controls.Add(this.label2);
			this.tabChooseOrganizeMethod.Location = new System.Drawing.Point(0, 0);
			this.tabChooseOrganizeMethod.Name = "tabChooseOrganizeMethod";
			this.tabChooseOrganizeMethod.Padding = new System.Windows.Forms.Padding(3);
			this.tabChooseOrganizeMethod.Size = new System.Drawing.Size(628, 288);
			this.tabChooseOrganizeMethod.TabIndex = 1;
			this.tabChooseOrganizeMethod.Text = "設定檔案分類方式";
			this.tabChooseOrganizeMethod.UseVisualStyleBackColor = true;
			// 
			// organizeSelectionControl1
			// 
			this.organizeSelectionControl1.BackColor = System.Drawing.Color.Transparent;
			this.organizeSelectionControl1.Location = new System.Drawing.Point(68, 97);
			this.organizeSelectionControl1.Name = "organizeSelectionControl1";
			this.organizeSelectionControl1.OrganizeBy = InfiniteStorage.OrganizeMethod.YearMonthDay;
			this.organizeSelectionControl1.Size = new System.Drawing.Size(436, 158);
			this.organizeSelectionControl1.TabIndex = 3;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label8.Location = new System.Drawing.Point(68, 77);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(484, 2);
			this.label8.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label2.Location = new System.Drawing.Point(12, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(596, 45);
			this.label2.TabIndex = 1;
			this.label2.Text = "不只是幫您安全把檔案送回電腦，還幫您按照日期收納，讓您快速找尋跟瀏覽";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabInstalledSuccess
			// 
			this.tabInstalledSuccess.Controls.Add(this.transferringControl1);
			this.tabInstalledSuccess.Location = new System.Drawing.Point(0, 0);
			this.tabInstalledSuccess.Name = "tabInstalledSuccess";
			this.tabInstalledSuccess.Padding = new System.Windows.Forms.Padding(3);
			this.tabInstalledSuccess.Size = new System.Drawing.Size(628, 288);
			this.tabInstalledSuccess.TabIndex = 2;
			this.tabInstalledSuccess.Text = "設定完成";
			this.tabInstalledSuccess.UseVisualStyleBackColor = true;
			// 
			// transferringControl1
			// 
			this.transferringControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.transferringControl1.Location = new System.Drawing.Point(3, 3);
			this.transferringControl1.Name = "transferringControl1";
			this.transferringControl1.Size = new System.Drawing.Size(622, 282);
			this.transferringControl1.TabIndex = 0;
			this.transferringControl1.WebSocketContext = null;
			// 
			// FirstUseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(628, 334);
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
			((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
			this.tabChooseLocation.ResumeLayout(false);
			this.tabChooseOrganizeMethod.ResumeLayout(false);
			this.tabInstalledSuccess.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabWelcomeAndWaitConnect;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabChooseOrganizeMethod;
		private System.Windows.Forms.TabPage tabInstalledSuccess;
		private System.Windows.Forms.Label hrLine;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private OrganizeSelectionControl organizeSelectionControl1;
		private System.Windows.Forms.Label instruction1;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.TabPage tabChooseLocation;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private StorageLocationControl storageLocationControl1;
		private TransferringControl transferringControl1;
	}
}