namespace Gui
{
	partial class InstallationLocationStep1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationLocationStep1));
			this.lblDescription = new System.Windows.Forms.Label();
			this.sdcbDiskSpace = new SharpSetup.UI.Controls.SimpleDiskCostBox();
			this.changeButton = new System.Windows.Forms.Button();
			this.location = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// lblDescription
			// 
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			// 
			// sdcbDiskSpace
			// 
			resources.ApplyResources(this.sdcbDiskSpace, "sdcbDiskSpace");
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Total);
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Free);
			this.sdcbDiskSpace.Name = "sdcbDiskSpace";
			// 
			// changeButton
			// 
			resources.ApplyResources(this.changeButton, "changeButton");
			this.changeButton.Name = "changeButton";
			this.changeButton.UseVisualStyleBackColor = true;
			this.changeButton.Click += new System.EventHandler(this.changeButton_Click);
			// 
			// location
			// 
			resources.ApplyResources(this.location, "location");
			this.location.Name = "location";
			// 
			// InstallationLocationStep1
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.location);
			this.Controls.Add(this.changeButton);
			this.Controls.Add(this.sdcbDiskSpace);
			this.Controls.Add(this.lblDescription);
			this.Name = "InstallationLocationStep1";
			this.MoveNext += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.InstallationLocationStep1_MoveNext);
			this.Entering += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.InstallationLocationStep1_Entering);
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.sdcbDiskSpace, 0);
			this.Controls.SetChildIndex(this.changeButton, 0);
			this.Controls.SetChildIndex(this.location, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblDescription;
		private SharpSetup.UI.Controls.SimpleDiskCostBox sdcbDiskSpace;
		private System.Windows.Forms.Button changeButton;
		private System.Windows.Forms.TextBox location;
	}
}
