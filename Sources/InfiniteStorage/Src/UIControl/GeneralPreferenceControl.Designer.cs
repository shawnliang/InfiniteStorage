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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralPreferenceControl));
			this.label6 = new System.Windows.Forms.Label();
			this.libraryName = new System.Windows.Forms.TextBox();
			this.storageLocationControl1 = new InfiniteStorage.StorageLocationControl();
			this.SuspendLayout();
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// libraryName
			// 
			resources.ApplyResources(this.libraryName, "libraryName");
			this.libraryName.Name = "libraryName";
			this.libraryName.TextChanged += new System.EventHandler(this.libraryName_TextChanged);
			// 
			// storageLocationControl1
			// 
			resources.ApplyResources(this.storageLocationControl1, "storageLocationControl1");
			this.storageLocationControl1.Name = "storageLocationControl1";
			this.storageLocationControl1.Station = null;
			this.storageLocationControl1.StoragePath = "C:\\Users\\shawnliang\\Infinite Storage";
			// 
			// GeneralPreferenceControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.Controls.Add(this.libraryName);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.storageLocationControl1);
			this.Name = "GeneralPreferenceControl";
			this.Load += new System.EventHandler(this.GeneralPreferenceControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private StorageLocationControl storageLocationControl1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox libraryName;
	}
}
