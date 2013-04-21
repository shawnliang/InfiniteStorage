namespace InfiniteStorage
{
	partial class OrganizeSelectionControl
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
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.radioYYYYMMDD = new System.Windows.Forms.RadioButton();
			this.radioYYYYMM = new System.Windows.Forms.RadioButton();
			this.radioYYYY = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label10.Location = new System.Drawing.Point(3, 35);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(529, 20);
			this.label10.TabIndex = 6;
			this.label10.Text = "我們將依照您設定的方式，按照檔案的拍攝、錄影、錄音時間，自動放入不同日期的資料夾";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label9.Location = new System.Drawing.Point(3, 10);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(128, 16);
			this.label9.TabIndex = 5;
			this.label9.Text = "請選擇分類方式：\r\n";
			// 
			// radioYYYYMMDD
			// 
			this.radioYYYYMMDD.AutoSize = true;
			this.radioYYYYMMDD.Location = new System.Drawing.Point(6, 58);
			this.radioYYYYMMDD.Name = "radioYYYYMMDD";
			this.radioYYYYMMDD.Size = new System.Drawing.Size(83, 17);
			this.radioYYYYMMDD.TabIndex = 7;
			this.radioYYYYMMDD.TabStop = true;
			this.radioYYYYMMDD.Text = "年 / 月 / 日";
			this.radioYYYYMMDD.UseVisualStyleBackColor = true;
			// 
			// radioYYYYMM
			// 
			this.radioYYYYMM.AutoSize = true;
			this.radioYYYYMM.Checked = true;
			this.radioYYYYMM.Location = new System.Drawing.Point(6, 81);
			this.radioYYYYMM.Name = "radioYYYYMM";
			this.radioYYYYMM.Size = new System.Drawing.Size(60, 17);
			this.radioYYYYMM.TabIndex = 8;
			this.radioYYYYMM.TabStop = true;
			this.radioYYYYMM.Text = "年 / 月";
			this.radioYYYYMM.UseVisualStyleBackColor = true;
			// 
			// radioYYYY
			// 
			this.radioYYYY.AutoSize = true;
			this.radioYYYY.Location = new System.Drawing.Point(6, 104);
			this.radioYYYY.Name = "radioYYYY";
			this.radioYYYY.Size = new System.Drawing.Size(37, 17);
			this.radioYYYY.TabIndex = 9;
			this.radioYYYY.TabStop = true;
			this.radioYYYY.Text = "年";
			this.radioYYYY.UseVisualStyleBackColor = true;
			// 
			// OrganizeSelectionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.radioYYYY);
			this.Controls.Add(this.radioYYYYMM);
			this.Controls.Add(this.radioYYYYMMDD);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Name = "OrganizeSelectionControl";
			this.Size = new System.Drawing.Size(535, 141);
			this.Load += new System.EventHandler(this.OrganizeSelectionControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton radioYYYYMMDD;
		private System.Windows.Forms.RadioButton radioYYYYMM;
		private System.Windows.Forms.RadioButton radioYYYY;
	}
}
