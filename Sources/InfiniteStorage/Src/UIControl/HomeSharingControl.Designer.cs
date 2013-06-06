namespace InfiniteStorage.Src.UIControl
{
	partial class HomeSharingControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeSharingControl));
			this.enableHomeSharing = new System.Windows.Forms.CheckBox();
			this.pwdProtection = new System.Windows.Forms.CheckBox();
			this.lblHomeSharingExplain = new System.Windows.Forms.Label();
			this.password = new System.Windows.Forms.TextBox();
			this.showPassword = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.invalidPwd = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// enableHomeSharing
			// 
			resources.ApplyResources(this.enableHomeSharing, "enableHomeSharing");
			this.enableHomeSharing.Checked = true;
			this.enableHomeSharing.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableHomeSharing.Name = "enableHomeSharing";
			this.enableHomeSharing.UseVisualStyleBackColor = true;
			this.enableHomeSharing.CheckedChanged += new System.EventHandler(this.enableHomeSharing_CheckedChanged);
			// 
			// pwdProtection
			// 
			resources.ApplyResources(this.pwdProtection, "pwdProtection");
			this.pwdProtection.Name = "pwdProtection";
			this.pwdProtection.UseVisualStyleBackColor = true;
			this.pwdProtection.CheckedChanged += new System.EventHandler(this.pwdProtection_CheckedChanged);
			// 
			// lblHomeSharingExplain
			// 
			resources.ApplyResources(this.lblHomeSharingExplain, "lblHomeSharingExplain");
			this.lblHomeSharingExplain.Name = "lblHomeSharingExplain";
			// 
			// password
			// 
			resources.ApplyResources(this.password, "password");
			this.password.Name = "password";
			this.password.TextChanged += new System.EventHandler(this.password_TextChanged);
			this.password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.password_KeyPress);
			// 
			// showPassword
			// 
			resources.ApplyResources(this.showPassword, "showPassword");
			this.showPassword.Name = "showPassword";
			this.showPassword.UseVisualStyleBackColor = true;
			this.showPassword.CheckedChanged += new System.EventHandler(this.showPassword_CheckedChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label1.Name = "label1";
			// 
			// invalidPwd
			// 
			resources.ApplyResources(this.invalidPwd, "invalidPwd");
			this.invalidPwd.ForeColor = System.Drawing.Color.Red;
			this.invalidPwd.Name = "invalidPwd";
			// 
			// HomeSharingControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.invalidPwd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.showPassword);
			this.Controls.Add(this.password);
			this.Controls.Add(this.lblHomeSharingExplain);
			this.Controls.Add(this.pwdProtection);
			this.Controls.Add(this.enableHomeSharing);
			this.Name = "HomeSharingControl";
			this.Load += new System.EventHandler(this.HomeSharingControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox enableHomeSharing;
		private System.Windows.Forms.CheckBox pwdProtection;
		private System.Windows.Forms.Label lblHomeSharingExplain;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.CheckBox showPassword;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label invalidPwd;
	}
}
