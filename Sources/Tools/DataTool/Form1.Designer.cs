﻿namespace DataTool
{
	partial class Form1
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
			this.generateButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.filesCountTxtBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBoxNotify = new System.Windows.Forms.TextBox();
			this.connectButton = new System.Windows.Forms.Button();
			this.disconnectButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// generateButton
			// 
			this.generateButton.Location = new System.Drawing.Point(12, 62);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(75, 23);
			this.generateButton.TabIndex = 0;
			this.generateButton.Text = "generate";
			this.generateButton.UseVisualStyleBackColor = true;
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Generate sample files";
			// 
			// filesCountTxtBox
			// 
			this.filesCountTxtBox.Location = new System.Drawing.Point(12, 36);
			this.filesCountTxtBox.Name = "filesCountTxtBox";
			this.filesCountTxtBox.Size = new System.Drawing.Size(218, 20);
			this.filesCountTxtBox.TabIndex = 2;
			this.filesCountTxtBox.Text = "20000";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 161);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Notify server";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 177);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(218, 20);
			this.textBox1.TabIndex = 4;
			this.textBox1.Text = "ws://127.0.0.1:13995/";
			// 
			// textBoxNotify
			// 
			this.textBoxNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxNotify.Location = new System.Drawing.Point(12, 203);
			this.textBoxNotify.Multiline = true;
			this.textBoxNotify.Name = "textBoxNotify";
			this.textBoxNotify.Size = new System.Drawing.Size(503, 208);
			this.textBoxNotify.TabIndex = 5;
			// 
			// connectButton
			// 
			this.connectButton.Location = new System.Drawing.Point(236, 175);
			this.connectButton.Name = "connectButton";
			this.connectButton.Size = new System.Drawing.Size(75, 23);
			this.connectButton.TabIndex = 6;
			this.connectButton.Text = "connect";
			this.connectButton.UseVisualStyleBackColor = true;
			this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
			// 
			// disconnectButton
			// 
			this.disconnectButton.Location = new System.Drawing.Point(317, 174);
			this.disconnectButton.Name = "disconnectButton";
			this.disconnectButton.Size = new System.Drawing.Size(75, 23);
			this.disconnectButton.TabIndex = 7;
			this.disconnectButton.Text = "disconnect";
			this.disconnectButton.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 423);
			this.Controls.Add(this.disconnectButton);
			this.Controls.Add(this.connectButton);
			this.Controls.Add(this.textBoxNotify);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.filesCountTxtBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.generateButton);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox filesCountTxtBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBoxNotify;
		private System.Windows.Forms.Button connectButton;
		private System.Windows.Forms.Button disconnectButton;
	}
}

