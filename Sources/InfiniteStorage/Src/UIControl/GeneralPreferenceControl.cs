using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class GeneralPreferenceControl : UserControl
	{
		public GeneralPreferenceControl()
		{
			InitializeComponent();
		}

		private void GeneralPreferenceControl_Load(object sender, EventArgs e)
		{
			lblComputerName.Text = Environment.MachineName;
		}
	}
}
