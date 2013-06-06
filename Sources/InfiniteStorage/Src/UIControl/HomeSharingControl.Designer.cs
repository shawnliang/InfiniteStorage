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
			this.enableHomeSharing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.enableHomeSharing.Checked = true;
			this.enableHomeSharing.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableHomeSharing.Location = new System.Drawing.Point(19, 19);
			this.enableHomeSharing.Name = "enableHomeSharing";
			this.enableHomeSharing.Size = new System.Drawing.Size(424, 24);
			this.enableHomeSharing.TabIndex = 0;
			this.enableHomeSharing.Text = "Enable Home Sharing";
			this.enableHomeSharing.UseVisualStyleBackColor = true;
			this.enableHomeSharing.CheckedChanged += new System.EventHandler(this.enableHomeSharing_CheckedChanged);
			// 
			// pwdProtection
			// 
			this.pwdProtection.AutoSize = true;
			this.pwdProtection.Location = new System.Drawing.Point(40, 79);
			this.pwdProtection.Name = "pwdProtection";
			this.pwdProtection.Size = new System.Drawing.Size(122, 17);
			this.pwdProtection.TabIndex = 1;
			this.pwdProtection.Text = "Password protection";
			this.pwdProtection.UseVisualStyleBackColor = true;
			this.pwdProtection.Visible = false;
			this.pwdProtection.CheckedChanged += new System.EventHandler(this.pwdProtection_CheckedChanged);
			// 
			// lblHomeSharingExplain
			// 
			this.lblHomeSharingExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblHomeSharingExplain.Location = new System.Drawing.Point(37, 46);
			this.lblHomeSharingExplain.Name = "lblHomeSharingExplain";
			this.lblHomeSharingExplain.Size = new System.Drawing.Size(406, 30);
			this.lblHomeSharingExplain.TabIndex = 2;
			this.lblHomeSharingExplain.Text = "By enabling Home Sharing, you can enjoy your favorite photos and home videos on A" +
    "ndroid pads and Google TV.";
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(59, 102);
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.Size = new System.Drawing.Size(202, 20);
			this.password.TabIndex = 3;
			this.password.Visible = false;
			this.password.TextChanged += new System.EventHandler(this.password_TextChanged);
			this.password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.password_KeyPress);
			// 
			// showPassword
			// 
			this.showPassword.AutoSize = true;
			this.showPassword.Location = new System.Drawing.Point(160, 127);
			this.showPassword.Name = "showPassword";
			this.showPassword.Size = new System.Drawing.Size(101, 17);
			this.showPassword.TabIndex = 4;
			this.showPassword.Text = "Show password";
			this.showPassword.UseVisualStyleBackColor = true;
			this.showPassword.Visible = false;
			this.showPassword.CheckedChanged += new System.EventHandler(this.showPassword_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label1.Location = new System.Drawing.Point(60, 128);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "4 digits, e.g., 4628";
			this.label1.Visible = false;
			// 
			// invalidPwd
			// 
			this.invalidPwd.AutoSize = true;
			this.invalidPwd.ForeColor = System.Drawing.Color.Red;
			this.invalidPwd.Location = new System.Drawing.Point(267, 105);
			this.invalidPwd.Name = "invalidPwd";
			this.invalidPwd.Size = new System.Drawing.Size(86, 13);
			this.invalidPwd.TabIndex = 6;
			this.invalidPwd.Text = "Invalid password";
			this.invalidPwd.Visible = false;
			// 
			// HomeSharingControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.invalidPwd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.showPassword);
			this.Controls.Add(this.password);
			this.Controls.Add(this.lblHomeSharingExplain);
			this.Controls.Add(this.pwdProtection);
			this.Controls.Add(this.enableHomeSharing);
			this.Name = "HomeSharingControl";
			this.Size = new System.Drawing.Size(467, 299);
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
