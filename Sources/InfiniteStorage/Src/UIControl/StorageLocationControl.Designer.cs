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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageLocationControl));
			this.changeStorageButton = new System.Windows.Forms.Button();
			this.storageLocationBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// changeStorageButton
			// 
			resources.ApplyResources(this.changeStorageButton, "changeStorageButton");
			this.changeStorageButton.Name = "changeStorageButton";
			this.changeStorageButton.UseVisualStyleBackColor = true;
			this.changeStorageButton.Click += new System.EventHandler(this.changeStorageButton_Click);
			// 
			// storageLocationBox
			// 
			resources.ApplyResources(this.storageLocationBox, "storageLocationBox");
			this.storageLocationBox.Name = "storageLocationBox";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label7.Name = "label7";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// StorageLocationControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.changeStorageButton);
			this.Controls.Add(this.storageLocationBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Name = "StorageLocationControl";
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
