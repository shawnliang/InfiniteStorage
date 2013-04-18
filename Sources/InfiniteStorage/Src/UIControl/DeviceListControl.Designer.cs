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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.devNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.delCol = new System.Windows.Forms.DataGridViewButtonColumn();
			this.label1 = new System.Windows.Forms.Label();
			this.bgPopulate = new System.ComponentModel.BackgroundWorker();
			this.rejectOtherDevices = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.devNameCol,
            this.delCol});
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.Location = new System.Drawing.Point(17, 49);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(601, 180);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
			// 
			// devNameCol
			// 
			this.devNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.devNameCol.DataPropertyName = "device_name";
			this.devNameCol.FillWeight = 70F;
			this.devNameCol.HeaderText = "裝置名稱";
			this.devNameCol.Name = "devNameCol";
			this.devNameCol.ReadOnly = true;
			// 
			// delCol
			// 
			this.delCol.HeaderText = "";
			this.delCol.Name = "delCol";
			this.delCol.ReadOnly = true;
			this.delCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.delCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.delCol.Text = "刪除";
			this.delCol.UseColumnTextForButtonValue = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "連結的手持裝置:";
			// 
			// bgPopulate
			// 
			this.bgPopulate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgPopulate_DoWork);
			this.bgPopulate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgPopulate_RunWorkerCompleted);
			// 
			// rejectOtherDevices
			// 
			this.rejectOtherDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.rejectOtherDevices.AutoSize = true;
			this.rejectOtherDevices.Location = new System.Drawing.Point(17, 246);
			this.rejectOtherDevices.Name = "rejectOtherDevices";
			this.rejectOtherDevices.Size = new System.Drawing.Size(350, 17);
			this.rejectOtherDevices.TabIndex = 2;
			this.rejectOtherDevices.Text = "僅允許以上裝置備份到此電腦，拒絕所有其他裝置的連線邀請";
			this.rejectOtherDevices.UseVisualStyleBackColor = true;
			this.rejectOtherDevices.CheckedChanged += new System.EventHandler(this.rejectOtherDevices_CheckedChanged);
			// 
			// DeviceListControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.rejectOtherDevices);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dataGridView1);
			this.Name = "DeviceListControl";
			this.Size = new System.Drawing.Size(650, 278);
			this.Load += new System.EventHandler(this.DeviceListControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.BackgroundWorker bgPopulate;
		private System.Windows.Forms.DataGridViewTextBoxColumn devNameCol;
		private System.Windows.Forms.DataGridViewButtonColumn delCol;
		private System.Windows.Forms.CheckBox rejectOtherDevices;
	}
}
