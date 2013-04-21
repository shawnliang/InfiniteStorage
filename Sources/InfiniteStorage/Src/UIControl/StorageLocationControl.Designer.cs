namespace InfiniteStorage
{
	partial class StorageLocationControl
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
			this.changeStorageButton = new System.Windows.Forms.Button();
			this.storageLocationBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// changeStorageButton
			// 
			this.changeStorageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeStorageButton.Location = new System.Drawing.Point(430, 60);
			this.changeStorageButton.Name = "changeStorageButton";
			this.changeStorageButton.Size = new System.Drawing.Size(75, 23);
			this.changeStorageButton.TabIndex = 9;
			this.changeStorageButton.Text = "變更...";
			this.changeStorageButton.UseVisualStyleBackColor = true;
			this.changeStorageButton.Click += new System.EventHandler(this.changeStorageButton_Click);
			// 
			// storageLocationBox
			// 
			this.storageLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.storageLocationBox.Enabled = false;
			this.storageLocationBox.Location = new System.Drawing.Point(6, 62);
			this.storageLocationBox.Name = "storageLocationBox";
			this.storageLocationBox.Size = new System.Drawing.Size(409, 20);
			this.storageLocationBox.TabIndex = 8;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label7.Location = new System.Drawing.Point(3, 36);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(379, 13);
			this.label7.TabIndex = 7;
			this.label7.Text = "您手機中的所有照片、影片、錄音當，都會自動存放到此資料夾底下：\r\n";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label6.Location = new System.Drawing.Point(3, 6);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 16);
			this.label6.TabIndex = 6;
			this.label6.Text = "請選擇備份位置：";
			// 
			// StorageLocationControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.changeStorageButton);
			this.Controls.Add(this.storageLocationBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Name = "StorageLocationControl";
			this.Size = new System.Drawing.Size(508, 95);
			this.Load += new System.EventHandler(this.StorageLocationControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button changeStorageButton;
		private System.Windows.Forms.TextBox storageLocationBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
	}
}
