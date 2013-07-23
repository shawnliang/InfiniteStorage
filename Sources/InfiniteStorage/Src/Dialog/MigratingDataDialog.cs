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
	public partial class MigratingDataDialog : Form
	{
		bool closeByApp;

		public MigratingDataDialog()
		{
			InitializeComponent();
			Icon = Resources.ProductIcon;
		}

		public void CloseByApp()
		{
			closeByApp = true;
			Close();
		}

		private void MigratingDataDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && !closeByApp)
			{
				e.Cancel = true;
			}
		}
	}
}
