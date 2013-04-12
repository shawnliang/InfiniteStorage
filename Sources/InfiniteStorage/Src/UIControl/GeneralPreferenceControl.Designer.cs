namespace InfiniteStorage
{
	partial class GeneralPreferenceControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.radioY = new System.Windows.Forms.RadioButton();
			this.radioYM = new System.Windows.Forms.RadioButton();
			this.radioYMD = new System.Windows.Forms.RadioButton();
			this.lblComputerName = new System.Windows.Forms.Label();
			this.backupLocationControl = new InfiniteStorage.BackupLocationControl();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "This computer :";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.radioY);
			this.groupBox2.Controls.Add(this.radioYM);
			this.groupBox2.Controls.Add(this.radioYMD);
			this.groupBox2.Location = new System.Drawing.Point(15, 186);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(573, 128);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "How files are organized?";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 28);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(280, 13);
			this.label6.TabIndex = 3;
			this.label6.Text = "Your photos and videos are backup to seperate folders by";
			// 
			// radioY
			// 
			this.radioY.AutoSize = true;
			this.radioY.Location = new System.Drawing.Point(9, 100);
			this.radioY.Name = "radioY";
			this.radioY.Size = new System.Drawing.Size(47, 17);
			this.radioY.TabIndex = 2;
			this.radioY.Text = "Year";
			this.radioY.UseVisualStyleBackColor = true;
			// 
			// radioYM
			// 
			this.radioYM.AutoSize = true;
			this.radioYM.Location = new System.Drawing.Point(9, 77);
			this.radioYM.Name = "radioYM";
			this.radioYM.Size = new System.Drawing.Size(88, 17);
			this.radioYM.TabIndex = 1;
			this.radioYM.Text = "Year \\ Month";
			this.radioYM.UseVisualStyleBackColor = true;
			// 
			// radioYMD
			// 
			this.radioYMD.AutoSize = true;
			this.radioYMD.Checked = true;
			this.radioYMD.Location = new System.Drawing.Point(9, 54);
			this.radioYMD.Name = "radioYMD";
			this.radioYMD.Size = new System.Drawing.Size(118, 17);
			this.radioYMD.TabIndex = 0;
			this.radioYMD.TabStop = true;
			this.radioYMD.Text = "Year \\ Month \\ Day";
			this.radioYMD.UseVisualStyleBackColor = true;
			// 
			// lblComputerName
			// 
			this.lblComputerName.AutoSize = true;
			this.lblComputerName.Location = new System.Drawing.Point(114, 15);
			this.lblComputerName.Name = "lblComputerName";
			this.lblComputerName.Size = new System.Drawing.Size(105, 13);
			this.lblComputerName.TabIndex = 4;
			this.lblComputerName.Text = "[this computer name]";
			// 
			// backupLocationControl
			// 
			this.backupLocationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.backupLocationControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.backupLocationControl.CustomAudioLocation = "C:\\Users\\shawnliang\\Podcasts\\Infinite Storage";
			this.backupLocationControl.CustomPhotoLocation = "C:\\Users\\shawnliang\\Pictures\\Infinite Storage";
			this.backupLocationControl.CustomVideoLocation = "C:\\Users\\shawnliang\\Videos\\Infinite Storage";
			this.backupLocationControl.Location = new System.Drawing.Point(15, 40);
			this.backupLocationControl.LocationType = InfiniteStorage.LocationType.SingleFolder;
			this.backupLocationControl.Name = "backupLocationControl";
			this.backupLocationControl.SingleFolderLocation = "C:\\Users\\shawnliang\\Infinite Storage";
			this.backupLocationControl.Size = new System.Drawing.Size(573, 140);
			this.backupLocationControl.TabIndex = 5;
			// 
			// GeneralPreferenceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.backupLocationControl);
			this.Controls.Add(this.lblComputerName);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label1);
			this.Name = "GeneralPreferenceControl";
			this.Size = new System.Drawing.Size(602, 334);
			this.Load += new System.EventHandler(this.GeneralPreferenceControl_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioY;
		private System.Windows.Forms.RadioButton radioYM;
		private System.Windows.Forms.RadioButton radioYMD;
		private System.Windows.Forms.Label lblComputerName;
		private BackupLocationControl backupLocationControl;
	}
}
