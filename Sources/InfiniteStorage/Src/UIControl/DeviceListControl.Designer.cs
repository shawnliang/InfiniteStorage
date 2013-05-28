namespace InfiniteStorage
{
	partial class DeviceListControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceListControl));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.label1 = new System.Windows.Forms.Label();
			this.bgPopulate = new System.ComponentModel.BackgroundWorker();
			this.label2 = new System.Windows.Forms.Label();
			this.radioAllow = new System.Windows.Forms.RadioButton();
			this.radioDisallow = new System.Windows.Forms.RadioButton();
			this.showBackupProgress = new System.Windows.Forms.CheckBox();
			this.devNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.delCol = new System.Windows.Forms.DataGridViewButtonColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.devNameCol,
            this.delCol});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// bgPopulate
			// 
			this.bgPopulate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgPopulate_DoWork);
			this.bgPopulate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgPopulate_RunWorkerCompleted);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// radioAllow
			// 
			resources.ApplyResources(this.radioAllow, "radioAllow");
			this.radioAllow.Checked = true;
			this.radioAllow.Name = "radioAllow";
			this.radioAllow.TabStop = true;
			this.radioAllow.UseVisualStyleBackColor = true;
			this.radioAllow.CheckedChanged += new System.EventHandler(this.rejectOtherDevices_CheckedChanged);
			// 
			// radioDisallow
			// 
			resources.ApplyResources(this.radioDisallow, "radioDisallow");
			this.radioDisallow.Name = "radioDisallow";
			this.radioDisallow.UseVisualStyleBackColor = true;
			this.radioDisallow.CheckedChanged += new System.EventHandler(this.rejectOtherDevices_CheckedChanged);
			// 
			// showBackupProgress
			// 
			resources.ApplyResources(this.showBackupProgress, "showBackupProgress");
			this.showBackupProgress.Name = "showBackupProgress";
			this.showBackupProgress.UseVisualStyleBackColor = true;
			this.showBackupProgress.Click += new System.EventHandler(this.showBackupProgress_Click);
			// 
			// devNameCol
			// 
			this.devNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.devNameCol.DataPropertyName = "device_name";
			this.devNameCol.FillWeight = 70F;
			resources.ApplyResources(this.devNameCol, "devNameCol");
			this.devNameCol.Name = "devNameCol";
			this.devNameCol.ReadOnly = true;
			// 
			// delCol
			// 
			resources.ApplyResources(this.delCol, "delCol");
			this.delCol.Name = "delCol";
			this.delCol.ReadOnly = true;
			this.delCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.delCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.delCol.Text = "刪除";
			this.delCol.UseColumnTextForButtonValue = true;
			// 
			// DeviceListControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.showBackupProgress);
			this.Controls.Add(this.radioDisallow);
			this.Controls.Add(this.radioAllow);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dataGridView1);
			this.Name = "DeviceListControl";
			this.Load += new System.EventHandler(this.DeviceListControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.BackgroundWorker bgPopulate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioAllow;
		private System.Windows.Forms.RadioButton radioDisallow;
		private System.Windows.Forms.CheckBox showBackupProgress;
		private System.Windows.Forms.DataGridViewTextBoxColumn devNameCol;
		private System.Windows.Forms.DataGridViewButtonColumn delCol;
	}
}
