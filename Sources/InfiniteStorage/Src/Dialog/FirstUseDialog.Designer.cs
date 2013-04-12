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
			this.label1 = new System.Windows.Forms.Label();
			this.tabChooseFolder = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.generalPreferenceControl1 = new InfiniteStorage.GeneralPreferenceControl();
			this.tabInstallAppAndStart = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.tabControlEx1.SuspendLayout();
			this.tabWelcome.SuspendLayout();
			this.tabChooseFolder.SuspendLayout();
			this.tabInstallAppAndStart.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(this.nextButton);
			this.panel1.Controls.Add(this.prevButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 375);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(707, 46);
			this.panel1.TabIndex = 0;
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nextButton.Location = new System.Drawing.Point(620, 6);
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
			this.tabControlEx1.Controls.Add(this.tabChooseFolder);
			this.tabControlEx1.Controls.Add(this.tabInstallAppAndStart);
			this.tabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlEx1.HideTabs = true;
			this.tabControlEx1.Location = new System.Drawing.Point(0, 0);
			this.tabControlEx1.Multiline = true;
			this.tabControlEx1.Name = "tabControlEx1";
			this.tabControlEx1.PageIndex = 2;
			this.tabControlEx1.SelectedIndex = 0;
			this.tabControlEx1.Size = new System.Drawing.Size(707, 375);
			this.tabControlEx1.TabIndex = 1;
			this.tabControlEx1.SelectedIndexChanged += new System.EventHandler(this.tabControlEx1_SelectedIndexChanged);
			// 
			// tabWelcome
			// 
			this.tabWelcome.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabWelcome.Controls.Add(this.label1);
			this.tabWelcome.Location = new System.Drawing.Point(0, 0);
			this.tabWelcome.Name = "tabWelcome";
			this.tabWelcome.Padding = new System.Windows.Forms.Padding(3);
			this.tabWelcome.Size = new System.Drawing.Size(707, 375);
			this.tabWelcome.TabIndex = 0;
			this.tabWelcome.Text = "Infinite Storage";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label1.Location = new System.Drawing.Point(156, 180);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(383, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "超屌備份讓你無痛備份手機和平板中的照片、錄音、短片";
			// 
			// tabChooseFolder
			// 
			this.tabChooseFolder.Controls.Add(this.label2);
			this.tabChooseFolder.Controls.Add(this.generalPreferenceControl1);
			this.tabChooseFolder.Location = new System.Drawing.Point(0, 0);
			this.tabChooseFolder.Name = "tabChooseFolder";
			this.tabChooseFolder.Padding = new System.Windows.Forms.Padding(3);
			this.tabChooseFolder.Size = new System.Drawing.Size(707, 375);
			this.tabChooseFolder.TabIndex = 1;
			this.tabChooseFolder.Text = "設定備份位置";
			this.tabChooseFolder.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label2.Location = new System.Drawing.Point(46, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(293, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "請先設定備份的檔案要放在何處以及如何放";
			// 
			// generalPreferenceControl1
			// 
			this.generalPreferenceControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.generalPreferenceControl1.Location = new System.Drawing.Point(44, 45);
			this.generalPreferenceControl1.Name = "generalPreferenceControl1";
			this.generalPreferenceControl1.OrganizeMethod = InfiniteStorage.OrganizeMethod.YearMonthDay;
			this.generalPreferenceControl1.Size = new System.Drawing.Size(618, 330);
			this.generalPreferenceControl1.TabIndex = 0;
			// 
			// tabInstallAppAndStart
			// 
			this.tabInstallAppAndStart.Controls.Add(this.label5);
			this.tabInstallAppAndStart.Controls.Add(this.label4);
			this.tabInstallAppAndStart.Controls.Add(this.label3);
			this.tabInstallAppAndStart.Location = new System.Drawing.Point(0, 0);
			this.tabInstallAppAndStart.Name = "tabInstallAppAndStart";
			this.tabInstallAppAndStart.Padding = new System.Windows.Forms.Padding(3);
			this.tabInstallAppAndStart.Size = new System.Drawing.Size(707, 375);
			this.tabInstallAppAndStart.TabIndex = 2;
			this.tabInstallAppAndStart.Text = "安裝 App";
			this.tabInstallAppAndStart.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(509, 241);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(65, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "[apple store]";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(124, 241);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(67, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "[google play]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			this.label3.Location = new System.Drawing.Point(121, 135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(453, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "好極了, 接下來在您的手機和平板上安裝超屌備份App, 一切就完成了!";
			// 
			// FirstUseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(707, 421);
			this.Controls.Add(this.tabControlEx1);
			this.Controls.Add(this.panel1);
			this.Name = "FirstUseDialog";
			this.Text = "[product name]";
			this.Load += new System.EventHandler(this.FirstUseDialog_Load);
			this.panel1.ResumeLayout(false);
			this.tabControlEx1.ResumeLayout(false);
			this.tabWelcome.ResumeLayout(false);
			this.tabWelcome.PerformLayout();
			this.tabChooseFolder.ResumeLayout(false);
			this.tabChooseFolder.PerformLayout();
			this.tabInstallAppAndStart.ResumeLayout(false);
			this.tabInstallAppAndStart.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private TabControlEx tabControlEx1;
		private System.Windows.Forms.TabPage tabWelcome;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button prevButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabChooseFolder;
		private System.Windows.Forms.Label label2;
		private GeneralPreferenceControl generalPreferenceControl1;
		private System.Windows.Forms.TabPage tabInstallAppAndStart;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
	}
}