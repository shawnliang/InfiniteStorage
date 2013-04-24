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
			this.tabWelcome = new System.Windows.Forms.TabPage();
			this.storageLocationControl1 = new InfiniteStorage.StorageLocationControl();
			this.hrLine = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabChooseOrganizeMethod = new System.Windows.Forms.TabPage();
			this.organizeSelectionControl1 = new InfiniteStorage.OrganizeSelectionControl();
			this.label8 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabInstallAppAndStart = new System.Windows.Forms.TabPage();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.tabControlEx1.SuspendLayout();
			this.tabWelcome.SuspendLayout();
			this.tabChooseOrganizeMethod.SuspendLayout();
			this.tabInstallAppAndStart.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			this.nextButton.Text = "Next";
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
			this.prevButton.Text = "Previous";
			this.prevButton.UseVisualStyleBackColor = true;
			this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
			// 
			// tabControlEx1
			// 
			this.tabControlEx1.Controls.Add(this.tabWelcome);
			this.tabControlEx1.Controls.Add(this.tabChooseOrganizeMethod);
			this.tabControlEx1.Controls.Add(this.tabInstallAppAndStart);
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
			// tabWelcome
			// 
			this.tabWelcome.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabWelcome.Controls.Add(this.storageLocationControl1);
			this.tabWelcome.Controls.Add(this.hrLine);
			this.tabWelcome.Controls.Add(this.label1);
			this.tabWelcome.Location = new System.Drawing.Point(0, 0);
			this.tabWelcome.Name = "tabWelcome";
			this.tabWelcome.Padding = new System.Windows.Forms.Padding(3);
			this.tabWelcome.Size = new System.Drawing.Size(628, 288);
			this.tabWelcome.TabIndex = 0;
			this.tabWelcome.Text = "Infinite Storage";
			// 
			// storageLocationControl1
			// 
			this.storageLocationControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.storageLocationControl1.Location = new System.Drawing.Point(68, 102);
			this.storageLocationControl1.Name = "storageLocationControl1";
			this.storageLocationControl1.Size = new System.Drawing.Size(506, 103);
			this.storageLocationControl1.StoragePath = "C:\\Users\\shawnliang\\Infinite Storage";
			this.storageLocationControl1.TabIndex = 2;
			// 
			// hrLine
			// 
			this.hrLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hrLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.hrLine.Location = new System.Drawing.Point(68, 77);
			this.hrLine.Name = "hrLine";
			this.hrLine.Size = new System.Drawing.Size(492, 2);
			this.hrLine.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label1.Location = new System.Drawing.Point(12, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(604, 45);
			this.label1.TabIndex = 0;
			this.label1.Text = "再也不怕手機照片、影片、錄音當遺書或手機空間不夠了\r\n只要電腦開著 檔案就會自己跑回來！";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
			this.tabChooseOrganizeMethod.Text = "設定備份位置";
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
			this.label8.Size = new System.Drawing.Size(492, 2);
			this.label8.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label2.Location = new System.Drawing.Point(12, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(604, 45);
			this.label2.TabIndex = 1;
			this.label2.Text = "不只是幫您安全把檔案送回電腦，還幫您按照日期收納，讓您快速找尋跟瀏覽";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabInstallAppAndStart
			// 
			this.tabInstallAppAndStart.Controls.Add(this.pictureBox3);
			this.tabInstallAppAndStart.Controls.Add(this.pictureBox2);
			this.tabInstallAppAndStart.Controls.Add(this.pictureBox1);
			this.tabInstallAppAndStart.Controls.Add(this.label3);
			this.tabInstallAppAndStart.Location = new System.Drawing.Point(0, 0);
			this.tabInstallAppAndStart.Name = "tabInstallAppAndStart";
			this.tabInstallAppAndStart.Padding = new System.Windows.Forms.Padding(3);
			this.tabInstallAppAndStart.Size = new System.Drawing.Size(628, 288);
			this.tabInstallAppAndStart.TabIndex = 2;
			this.tabInstallAppAndStart.Text = "安裝 App";
			this.tabInstallAppAndStart.UseVisualStyleBackColor = true;
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox3.Image = global::InfiniteStorage.Properties.Resources.temp_app_logo;
			this.pictureBox3.Location = new System.Drawing.Point(228, 28);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(173, 103);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox3.TabIndex = 5;
			this.pictureBox3.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::InfiniteStorage.Properties.Resources.app_store_badge;
			this.pictureBox2.Location = new System.Drawing.Point(60, 195);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(198, 60);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 4;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.getItOnAppStore_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::InfiniteStorage.Properties.Resources.get_it_on_google_play;
			this.pictureBox1.Location = new System.Drawing.Point(404, 195);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(172, 60);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.getItOnGooglePlay_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold);
			this.label3.Location = new System.Drawing.Point(12, 150);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(604, 42);
			this.label3.TabIndex = 0;
			this.label3.Text = "趕緊在手機上裝起 app，馬上把照片、影片、 錄音檔送回來吧！";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FirstUseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(628, 334);
			this.Controls.Add(this.tabControlEx1);
			this.Controls.Add(this.panel1);
			this.Name = "FirstUseDialog";
			this.Text = "[product name]";
			this.Load += new System.EventHandler(this.FirstUseDialog_Load);
			this.panel1.ResumeLayout(false);
			this.tabControlEx1.ResumeLayout(false);
			this.tabWelcome.ResumeLayout(false);
			this.tabChooseOrganizeMethod.ResumeLayout(false);
			this.tabInstallAppAndStart.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabWelcome;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabChooseOrganizeMethod;
		private System.Windows.Forms.TabPage tabInstallAppAndStart;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label hrLine;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private OrganizeSelectionControl organizeSelectionControl1;
		private StorageLocationControl storageLocationControl1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
	}
}