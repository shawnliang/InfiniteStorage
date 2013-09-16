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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferenceDialog));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.checkboxAutoRun = new System.Windows.Forms.CheckBox();
			this.generalPreferenceControl1 = new InfiniteStorage.GeneralPreferenceControl();
			this.aboutControl1 = new InfiniteStorage.AboutControl();
			this.tabControl.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.tabAbout.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.tabGeneral);
			this.tabControl.Controls.Add(this.tabAbout);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			// 
			// tabGeneral
			// 
			resources.ApplyResources(this.tabGeneral, "tabGeneral");
			this.tabGeneral.Controls.Add(this.generalPreferenceControl1);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// tabAbout
			// 
			resources.ApplyResources(this.tabAbout, "tabAbout");
			this.tabAbout.Controls.Add(this.aboutControl1);
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonApply
			// 
			resources.ApplyResources(this.buttonApply, "buttonApply");
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// checkboxAutoRun
			// 
			resources.ApplyResources(this.checkboxAutoRun, "checkboxAutoRun");
			this.checkboxAutoRun.Name = "checkboxAutoRun";
			this.checkboxAutoRun.UseVisualStyleBackColor = true;
			this.checkboxAutoRun.Click += new System.EventHandler(this.handleAnySettingChanged);
			// 
			// generalPreferenceControl1
			// 
			resources.ApplyResources(this.generalPreferenceControl1, "generalPreferenceControl1");
			this.generalPreferenceControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.generalPreferenceControl1.Name = "generalPreferenceControl1";
			this.generalPreferenceControl1.Station = null;
			// 
			// aboutControl1
			// 
			resources.ApplyResources(this.aboutControl1, "aboutControl1");
			this.aboutControl1.LogLevel = InfiniteStorage.DebugLevel.WARN;
			this.aboutControl1.Name = "aboutControl1";
			// 
			// PreferenceDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.checkboxAutoRun);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PreferenceDialog";
			this.Load += new System.EventHandler(this.PreferenceDialog_Load);
			this.tabControl.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.tabAbout.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.CheckBox checkboxAutoRun;
		private GeneralPreferenceControl generalPreferenceControl1;
		private System.Windows.Forms.TabPage tabAbout;
		private AboutControl aboutControl1;
	}
}

