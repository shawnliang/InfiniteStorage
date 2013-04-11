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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.changeAudioLocation = new System.Windows.Forms.Button();
			this.changeVedioLocation = new System.Windows.Forms.Button();
			this.boxAudioLocation = new System.Windows.Forms.TextBox();
			this.boxVideoLocation = new System.Windows.Forms.TextBox();
			this.boxPhotoLocation = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.changePhotoLocation = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.radioY = new System.Windows.Forms.RadioButton();
			this.radioYM = new System.Windows.Forms.RadioButton();
			this.radioYMD = new System.Windows.Forms.RadioButton();
			this.lblComputerName = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
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
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.changeAudioLocation);
			this.groupBox1.Controls.Add(this.changeVedioLocation);
			this.groupBox1.Controls.Add(this.boxAudioLocation);
			this.groupBox1.Controls.Add(this.boxVideoLocation);
			this.groupBox1.Controls.Add(this.boxPhotoLocation);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.changePhotoLocation);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(15, 44);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(573, 117);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Backup location";
			// 
			// changeAudioLocation
			// 
			this.changeAudioLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeAudioLocation.Location = new System.Drawing.Point(465, 79);
			this.changeAudioLocation.Name = "changeAudioLocation";
			this.changeAudioLocation.Size = new System.Drawing.Size(87, 23);
			this.changeAudioLocation.TabIndex = 13;
			this.changeAudioLocation.Text = "Change...";
			this.changeAudioLocation.UseVisualStyleBackColor = true;
			this.changeAudioLocation.Click += new System.EventHandler(this.changeLocation_Click);
			// 
			// changeVedioLocation
			// 
			this.changeVedioLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeVedioLocation.Location = new System.Drawing.Point(465, 50);
			this.changeVedioLocation.Name = "changeVedioLocation";
			this.changeVedioLocation.Size = new System.Drawing.Size(87, 23);
			this.changeVedioLocation.TabIndex = 12;
			this.changeVedioLocation.Text = "Change...";
			this.changeVedioLocation.UseVisualStyleBackColor = true;
			this.changeVedioLocation.Click += new System.EventHandler(this.changeLocation_Click);
			// 
			// boxAudioLocation
			// 
			this.boxAudioLocation.Location = new System.Drawing.Point(54, 81);
			this.boxAudioLocation.Name = "boxAudioLocation";
			this.boxAudioLocation.Size = new System.Drawing.Size(387, 20);
			this.boxAudioLocation.TabIndex = 11;
			// 
			// boxVideoLocation
			// 
			this.boxVideoLocation.Location = new System.Drawing.Point(55, 52);
			this.boxVideoLocation.Name = "boxVideoLocation";
			this.boxVideoLocation.Size = new System.Drawing.Size(387, 20);
			this.boxVideoLocation.TabIndex = 10;
			// 
			// boxPhotoLocation
			// 
			this.boxPhotoLocation.Location = new System.Drawing.Point(55, 23);
			this.boxPhotoLocation.Name = "boxPhotoLocation";
			this.boxPhotoLocation.Size = new System.Drawing.Size(387, 20);
			this.boxPhotoLocation.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 84);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Audios:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Videos:";
			// 
			// changePhotoLocation
			// 
			this.changePhotoLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changePhotoLocation.Location = new System.Drawing.Point(465, 21);
			this.changePhotoLocation.Name = "changePhotoLocation";
			this.changePhotoLocation.Size = new System.Drawing.Size(87, 23);
			this.changePhotoLocation.TabIndex = 5;
			this.changePhotoLocation.Text = "Change...";
			this.changePhotoLocation.UseVisualStyleBackColor = true;
			this.changePhotoLocation.Click += new System.EventHandler(this.changeLocation_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Photos:";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.radioY);
			this.groupBox2.Controls.Add(this.radioYM);
			this.groupBox2.Controls.Add(this.radioYMD);
			this.groupBox2.Location = new System.Drawing.Point(15, 177);
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
			// GeneralPreferenceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblComputerName);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "GeneralPreferenceControl";
			this.Size = new System.Drawing.Size(602, 326);
			this.Load += new System.EventHandler(this.GeneralPreferenceControl_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button changePhotoLocation;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioY;
		private System.Windows.Forms.RadioButton radioYM;
		private System.Windows.Forms.RadioButton radioYMD;
		private System.Windows.Forms.Label lblComputerName;
		private System.Windows.Forms.Button changeAudioLocation;
		private System.Windows.Forms.Button changeVedioLocation;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox boxAudioLocation;
		private System.Windows.Forms.TextBox boxVideoLocation;
		private System.Windows.Forms.TextBox boxPhotoLocation;
	}
}
