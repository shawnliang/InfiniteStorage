namespace InfiniteStorage
{
	partial class BackupLocationControl
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.radioCustom = new System.Windows.Forms.RadioButton();
			this.radioSingleFolder = new System.Windows.Forms.RadioButton();
			this.radioMediaLibrary = new System.Windows.Forms.RadioButton();
			this.tabControlEx1 = new InfiniteStorage.TabControlEx();
			this.tabMediaLibrary = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.tabSingleFolder = new System.Windows.Forms.TabPage();
			this.changeSingleFolderButton = new System.Windows.Forms.Button();
			this.boxSingleFolder = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabCustom = new System.Windows.Forms.TabPage();
			this.changeAudioLocation = new System.Windows.Forms.Button();
			this.changeVideoLocation = new System.Windows.Forms.Button();
			this.changePhotoLocation = new System.Windows.Forms.Button();
			this.lblAudioLocation = new System.Windows.Forms.Label();
			this.lblVideoLocation = new System.Windows.Forms.Label();
			this.lblPhotoLocation = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControlEx1.SuspendLayout();
			this.tabMediaLibrary.SuspendLayout();
			this.tabSingleFolder.SuspendLayout();
			this.tabCustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.splitContainer1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(610, 162);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "檔案存放位置";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(3, 16);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.splitContainer1.Panel1.Controls.Add(this.radioCustom);
			this.splitContainer1.Panel1.Controls.Add(this.radioSingleFolder);
			this.splitContainer1.Panel1.Controls.Add(this.radioMediaLibrary);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControlEx1);
			this.splitContainer1.Size = new System.Drawing.Size(604, 143);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.TabIndex = 0;
			// 
			// radioCustom
			// 
			this.radioCustom.AutoSize = true;
			this.radioCustom.Location = new System.Drawing.Point(4, 92);
			this.radioCustom.Name = "radioCustom";
			this.radioCustom.Size = new System.Drawing.Size(49, 17);
			this.radioCustom.TabIndex = 2;
			this.radioCustom.Text = "自訂";
			this.radioCustom.UseVisualStyleBackColor = true;
			this.radioCustom.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// radioSingleFolder
			// 
			this.radioSingleFolder.AutoSize = true;
			this.radioSingleFolder.Checked = true;
			this.radioSingleFolder.Location = new System.Drawing.Point(4, 12);
			this.radioSingleFolder.Name = "radioSingleFolder";
			this.radioSingleFolder.Size = new System.Drawing.Size(169, 17);
			this.radioSingleFolder.TabIndex = 1;
			this.radioSingleFolder.TabStop = true;
			this.radioSingleFolder.Text = "全部檔案放在單一資料夾中";
			this.radioSingleFolder.UseVisualStyleBackColor = true;
			this.radioSingleFolder.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// radioMediaLibrary
			// 
			this.radioMediaLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.radioMediaLibrary.Location = new System.Drawing.Point(4, 44);
			this.radioMediaLibrary.Name = "radioMediaLibrary";
			this.radioMediaLibrary.Size = new System.Drawing.Size(194, 35);
			this.radioMediaLibrary.TabIndex = 0;
			this.radioMediaLibrary.Text = "檔案放在媒體櫃中";
			this.radioMediaLibrary.UseVisualStyleBackColor = true;
			this.radioMediaLibrary.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// tabControlEx1
			// 
			this.tabControlEx1.Controls.Add(this.tabMediaLibrary);
			this.tabControlEx1.Controls.Add(this.tabSingleFolder);
			this.tabControlEx1.Controls.Add(this.tabCustom);
			this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 3;
			this.tabControlEx1.SelectedIndex = 0;
			this.tabControlEx1.Size = new System.Drawing.Size(400, 143);
			this.tabControlEx1.TabIndex = 0;
			// 
			// tabMediaLibrary
			// 
			this.tabMediaLibrary.Controls.Add(this.label2);
			this.tabMediaLibrary.Location = new System.Drawing.Point(0, 0);
			this.tabMediaLibrary.Name = "tabMediaLibrary";
			this.tabMediaLibrary.Padding = new System.Windows.Forms.Padding(3);
			this.tabMediaLibrary.Size = new System.Drawing.Size(400, 143);
			this.tabMediaLibrary.TabIndex = 0;
			this.tabMediaLibrary.Text = "tabPage1";
			this.tabMediaLibrary.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(7, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(376, 114);
			this.label2.TabIndex = 0;
			this.label2.Text = "照片、短片、錄音檔分別放在我的圖片、我的影音和我的podcasts中，其他的多媒體應用程式會很容易地找到您所備份的檔案\r\n";
			// 
			// tabSingleFolder
			// 
			this.tabSingleFolder.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabSingleFolder.Controls.Add(this.changeSingleFolderButton);
			this.tabSingleFolder.Controls.Add(this.boxSingleFolder);
			this.tabSingleFolder.Controls.Add(this.label1);
			this.tabSingleFolder.Location = new System.Drawing.Point(0, 0);
			this.tabSingleFolder.Name = "tabSingleFolder";
			this.tabSingleFolder.Padding = new System.Windows.Forms.Padding(3);
			this.tabSingleFolder.Size = new System.Drawing.Size(400, 143);
			this.tabSingleFolder.TabIndex = 1;
			this.tabSingleFolder.Text = "tabPage2";
			// 
			// changeSingleFolderButton
			// 
			this.changeSingleFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeSingleFolderButton.Location = new System.Drawing.Point(319, 50);
			this.changeSingleFolderButton.Name = "changeSingleFolderButton";
			this.changeSingleFolderButton.Size = new System.Drawing.Size(75, 23);
			this.changeSingleFolderButton.TabIndex = 2;
			this.changeSingleFolderButton.Text = "變更...";
			this.changeSingleFolderButton.UseVisualStyleBackColor = true;
			this.changeSingleFolderButton.Click += new System.EventHandler(this.changeSingleFolderButton_Click);
			// 
			// boxSingleFolder
			// 
			this.boxSingleFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.boxSingleFolder.Enabled = false;
			this.boxSingleFolder.Location = new System.Drawing.Point(10, 52);
			this.boxSingleFolder.Name = "boxSingleFolder";
			this.boxSingleFolder.Size = new System.Drawing.Size(303, 20);
			this.boxSingleFolder.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(7, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(387, 37);
			this.label1.TabIndex = 0;
			this.label1.Text = "全部類型的檔案放在單一資料夾下,再 依照時間日期分不同資料夾歸檔放置:";
			// 
			// tabCustom
			// 
			this.tabCustom.Controls.Add(this.changeAudioLocation);
			this.tabCustom.Controls.Add(this.changeVideoLocation);
			this.tabCustom.Controls.Add(this.changePhotoLocation);
			this.tabCustom.Controls.Add(this.lblAudioLocation);
			this.tabCustom.Controls.Add(this.lblVideoLocation);
			this.tabCustom.Controls.Add(this.lblPhotoLocation);
			this.tabCustom.Controls.Add(this.label5);
			this.tabCustom.Controls.Add(this.label4);
			this.tabCustom.Controls.Add(this.label3);
			this.tabCustom.Location = new System.Drawing.Point(0, 0);
			this.tabCustom.Name = "tabCustom";
			this.tabCustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabCustom.Size = new System.Drawing.Size(400, 143);
			this.tabCustom.TabIndex = 2;
			this.tabCustom.Text = "tabPage3";
			this.tabCustom.UseVisualStyleBackColor = true;
			// 
			// changeAudioLocation
			// 
			this.changeAudioLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeAudioLocation.Location = new System.Drawing.Point(319, 89);
			this.changeAudioLocation.Name = "changeAudioLocation";
			this.changeAudioLocation.Size = new System.Drawing.Size(75, 23);
			this.changeAudioLocation.TabIndex = 8;
			this.changeAudioLocation.Text = "變更...";
			this.changeAudioLocation.UseVisualStyleBackColor = true;
			this.changeAudioLocation.Click += new System.EventHandler(this.changeCustomLocation_Click);
			// 
			// changeVideoLocation
			// 
			this.changeVideoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeVideoLocation.Location = new System.Drawing.Point(319, 50);
			this.changeVideoLocation.Name = "changeVideoLocation";
			this.changeVideoLocation.Size = new System.Drawing.Size(75, 23);
			this.changeVideoLocation.TabIndex = 7;
			this.changeVideoLocation.Text = "變更...";
			this.changeVideoLocation.UseVisualStyleBackColor = true;
			this.changeVideoLocation.Click += new System.EventHandler(this.changeCustomLocation_Click);
			// 
			// changePhotoLocation
			// 
			this.changePhotoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changePhotoLocation.Location = new System.Drawing.Point(319, 9);
			this.changePhotoLocation.Name = "changePhotoLocation";
			this.changePhotoLocation.Size = new System.Drawing.Size(75, 23);
			this.changePhotoLocation.TabIndex = 6;
			this.changePhotoLocation.Text = "變更...";
			this.changePhotoLocation.UseVisualStyleBackColor = true;
			this.changePhotoLocation.Click += new System.EventHandler(this.changeCustomLocation_Click);
			// 
			// lblAudioLocation
			// 
			this.lblAudioLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblAudioLocation.Location = new System.Drawing.Point(44, 96);
			this.lblAudioLocation.Name = "lblAudioLocation";
			this.lblAudioLocation.Size = new System.Drawing.Size(269, 30);
			this.lblAudioLocation.TabIndex = 5;
			this.lblAudioLocation.Text = "[....]";
			// 
			// lblVideoLocation
			// 
			this.lblVideoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblVideoLocation.Location = new System.Drawing.Point(44, 53);
			this.lblVideoLocation.Name = "lblVideoLocation";
			this.lblVideoLocation.Size = new System.Drawing.Size(269, 30);
			this.lblVideoLocation.TabIndex = 4;
			this.lblVideoLocation.Text = "[....]";
			// 
			// lblPhotoLocation
			// 
			this.lblPhotoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPhotoLocation.Location = new System.Drawing.Point(44, 12);
			this.lblPhotoLocation.Name = "lblPhotoLocation";
			this.lblPhotoLocation.Size = new System.Drawing.Size(269, 30);
			this.lblPhotoLocation.TabIndex = 3;
			this.lblPhotoLocation.Text = "[....]";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(7, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(31, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "錄音";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 53);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "短片";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "照片";
			// 
			// BackupLocationControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.groupBox1);
			this.Name = "BackupLocationControl";
			this.Size = new System.Drawing.Size(610, 162);
			this.Load += new System.EventHandler(this.BackupLocationControl_Load);
			this.groupBox1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControlEx1.ResumeLayout(false);
			this.tabMediaLibrary.ResumeLayout(false);
			this.tabSingleFolder.ResumeLayout(false);
			this.tabSingleFolder.PerformLayout();
			this.tabCustom.ResumeLayout(false);
			this.tabCustom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.RadioButton radioCustom;
		private System.Windows.Forms.RadioButton radioSingleFolder;
		private System.Windows.Forms.RadioButton radioMediaLibrary;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabMediaLibrary;
		private System.Windows.Forms.TabPage tabSingleFolder;
		private System.Windows.Forms.TabPage tabCustom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button changeSingleFolderButton;
		private System.Windows.Forms.TextBox boxSingleFolder;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblAudioLocation;
		private System.Windows.Forms.Label lblVideoLocation;
		private System.Windows.Forms.Label lblPhotoLocation;
		private System.Windows.Forms.Button changeAudioLocation;
		private System.Windows.Forms.Button changeVideoLocation;
		private System.Windows.Forms.Button changePhotoLocation;
	}
}
