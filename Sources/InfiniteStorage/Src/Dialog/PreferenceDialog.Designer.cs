namespace InfiniteStorage
{
	partial class PreferenceDialog
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
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.generalPreferenceControl1 = new InfiniteStorage.GeneralPreferenceControl();
			this.tabDevices = new System.Windows.Forms.TabPage();
			this.deviceListControl = new InfiniteStorage.DeviceListControl();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.checkboxAutoRun = new System.Windows.Forms.CheckBox();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabDevices.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabDevices);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(628, 356);
			this.tabControl.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.generalPreferenceControl1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(620, 330);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// generalPreferenceControl1
			// 
			this.generalPreferenceControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.generalPreferenceControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.generalPreferenceControl1.Enabled = false;
			this.generalPreferenceControl1.Location = new System.Drawing.Point(3, 3);
			this.generalPreferenceControl1.Name = "generalPreferenceControl1";
			this.generalPreferenceControl1.OrganizeMethod = InfiniteStorage.OrganizeMethod.YearMonthDay;
			this.generalPreferenceControl1.Size = new System.Drawing.Size(614, 324);
			this.generalPreferenceControl1.TabIndex = 0;
			// 
			// tabDevices
			// 
			this.tabDevices.Controls.Add(this.deviceListControl);
			this.tabDevices.Location = new System.Drawing.Point(4, 22);
			this.tabDevices.Name = "tabDevices";
			this.tabDevices.Padding = new System.Windows.Forms.Padding(3);
			this.tabDevices.Size = new System.Drawing.Size(620, 330);
			this.tabDevices.TabIndex = 1;
			this.tabDevices.Text = "Linked Devices";
			this.tabDevices.UseVisualStyleBackColor = true;
			// 
			// deviceListControl
			// 
			this.deviceListControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.deviceListControl.Location = new System.Drawing.Point(3, 3);
			this.deviceListControl.Name = "deviceListControl";
			this.deviceListControl.RejectOtherDevices = false;
			this.deviceListControl.Size = new System.Drawing.Size(186, 324);
			this.deviceListControl.TabIndex = 0;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(403, 374);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(484, 374);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonApply
			// 
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonApply.Enabled = false;
			this.buttonApply.Location = new System.Drawing.Point(565, 374);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 3;
			this.buttonApply.Text = "Apply";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// checkboxAutoRun
			// 
			this.checkboxAutoRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkboxAutoRun.AutoSize = true;
			this.checkboxAutoRun.Location = new System.Drawing.Point(16, 378);
			this.checkboxAutoRun.Name = "checkboxAutoRun";
			this.checkboxAutoRun.Size = new System.Drawing.Size(141, 17);
			this.checkboxAutoRun.TabIndex = 4;
			this.checkboxAutoRun.Text = "Auto run at system starts";
			this.checkboxAutoRun.UseVisualStyleBackColor = true;
			this.checkboxAutoRun.Click += new System.EventHandler(this.handleAnySettingChanged);
			// 
			// PreferenceDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(652, 406);
			this.Controls.Add(this.checkboxAutoRun);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PreferenceDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "[ProductName]";
			this.Load += new System.EventHandler(this.PreferenceDialog_Load);
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabDevices.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.CheckBox checkboxAutoRun;
		private GeneralPreferenceControl generalPreferenceControl1;
		private System.Windows.Forms.TabPage tabDevices;
		private DeviceListControl deviceListControl;
	}
}

