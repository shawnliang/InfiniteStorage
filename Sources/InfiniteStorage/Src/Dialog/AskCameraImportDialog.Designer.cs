namespace InfiniteStorage
{
	partial class AskCameraImportDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.devName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioDoNothing = new System.Windows.Forms.RadioButton();
			this.radioImport = new System.Windows.Forms.RadioButton();
			this.checkBoxRemember = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(411, 60);
			this.label1.TabIndex = 0;
			this.label1.Text = "Piary Photos automatically imports photos and videos from your digital camera. Y" +
    "ou can then organize your photos and videos and even share them with your friend" +
	"s with Piary Photos.";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(348, 288);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 30);
			this.button1.TabIndex = 1;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(267, 288);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 30);
			this.button2.TabIndex = 2;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::InfiniteStorage.Properties.Resources.photography_camera;
			this.pictureBox1.Location = new System.Drawing.Point(29, 91);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(23, 26);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(55, 98);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(97, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Connected device:";
			// 
			// devName
			// 
			this.devName.AutoSize = true;
			this.devName.Location = new System.Drawing.Point(158, 98);
			this.devName.Name = "devName";
			this.devName.Size = new System.Drawing.Size(60, 13);
			this.devName.TabIndex = 5;
			this.devName.Text = "[dev name]";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioDoNothing);
			this.groupBox1.Controls.Add(this.radioImport);
			this.groupBox1.Location = new System.Drawing.Point(15, 133);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(408, 89);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "What action do you want to take?";
			// 
			// radioDoNothing
			// 
			this.radioDoNothing.AutoSize = true;
			this.radioDoNothing.Location = new System.Drawing.Point(14, 51);
			this.radioDoNothing.Name = "radioDoNothing";
			this.radioDoNothing.Size = new System.Drawing.Size(77, 17);
			this.radioDoNothing.TabIndex = 1;
			this.radioDoNothing.TabStop = true;
			this.radioDoNothing.Text = "Do nothing";
			this.radioDoNothing.UseVisualStyleBackColor = true;
			// 
			// radioImport
			// 
			this.radioImport.AutoSize = true;
			this.radioImport.Location = new System.Drawing.Point(14, 27);
			this.radioImport.Name = "radioImport";
			this.radioImport.Size = new System.Drawing.Size(107, 17);
			this.radioImport.TabIndex = 0;
			this.radioImport.TabStop = true;
			this.radioImport.Text = "Import to Piary Photos";
			this.radioImport.UseVisualStyleBackColor = true;
			// 
			// checkBoxRemember
			// 
			this.checkBoxRemember.AutoSize = true;
			this.checkBoxRemember.Location = new System.Drawing.Point(15, 241);
			this.checkBoxRemember.Name = "checkBoxRemember";
			this.checkBoxRemember.Size = new System.Drawing.Size(232, 17);
			this.checkBoxRemember.TabIndex = 7;
			this.checkBoxRemember.Text = "Remember my settings. Don\'t ask me again.";
			this.checkBoxRemember.UseVisualStyleBackColor = true;
			// 
			// AskCameraImportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(435, 330);
			this.Controls.Add(this.checkBoxRemember);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.devName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AskCameraImportDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Piary Photos Auto Import";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.AskCameraImportDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label devName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioDoNothing;
		private System.Windows.Forms.RadioButton radioImport;
		private System.Windows.Forms.CheckBox checkBoxRemember;
	}
}