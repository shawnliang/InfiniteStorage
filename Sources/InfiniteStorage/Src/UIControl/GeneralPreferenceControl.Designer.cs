namespace InfiniteStorage
{
	partial class GeneralPreferenceControl
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
			this.organizeSelectionControl1 = new InfiniteStorage.OrganizeSelectionControl();
			this.storageLocationControl1 = new InfiniteStorage.StorageLocationControl();
			this.SuspendLayout();
			// 
			// organizeSelectionControl1
			// 
			this.organizeSelectionControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.organizeSelectionControl1.BackColor = System.Drawing.Color.Transparent;
			this.organizeSelectionControl1.Enabled = false;
			this.organizeSelectionControl1.Location = new System.Drawing.Point(14, 126);
			this.organizeSelectionControl1.Name = "organizeSelectionControl1";
			this.organizeSelectionControl1.OrganizeBy = InfiniteStorage.OrganizeMethod.YearMonth;
			this.organizeSelectionControl1.Size = new System.Drawing.Size(535, 141);
			this.organizeSelectionControl1.TabIndex = 6;
			// 
			// storageLocationControl1
			// 
			this.storageLocationControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.storageLocationControl1.Enabled = false;
			this.storageLocationControl1.Location = new System.Drawing.Point(14, 13);
			this.storageLocationControl1.Name = "storageLocationControl1";
			this.storageLocationControl1.Size = new System.Drawing.Size(535, 95);
			this.storageLocationControl1.StoragePath = "C:\\Users\\shawnliang\\Infinite Storage";
			this.storageLocationControl1.TabIndex = 5;
			// 
			// GeneralPreferenceControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.organizeSelectionControl1);
			this.Controls.Add(this.storageLocationControl1);
			this.Name = "GeneralPreferenceControl";
			this.Size = new System.Drawing.Size(568, 288);
			this.ResumeLayout(false);

		}

		#endregion

		private StorageLocationControl storageLocationControl1;
		private OrganizeSelectionControl organizeSelectionControl1;
	}
}
