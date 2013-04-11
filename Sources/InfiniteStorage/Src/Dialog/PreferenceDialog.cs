using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class PreferenceDialog : Form
	{
		public PreferenceDialog()
		{
			InitializeComponent();
			Text = Resources.ProductName;
			Icon = Resources.product_icon;
		}

		private void PreferenceDialog_Load(object sender, EventArgs e)
		{
			generalPreferenceControl1.PhotoLocation = Settings.Default.PhotoLocation;
			generalPreferenceControl1.VideoLocation = Settings.Default.VideoLocation;
			generalPreferenceControl1.AudioLocation = Settings.Default.AudioLocation;
			generalPreferenceControl1.OrganizeMethod = (OrganizeMethod)Settings.Default.OrganizeMethod;
		}
	}
}
