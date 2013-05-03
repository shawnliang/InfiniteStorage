using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public partial class PairingRequestDialog : Form
	{
		private ProtocolContext ctx;

		public PairingRequestDialog(ProtocolContext ctx)
		{
			this.ctx = ctx;
			InitializeComponent();
			Text = Resources.ProductName;
			Icon = Resources.product_icon;
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void PairingRequestDialog_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				questionLabel.Text = string.Format(Resources.AllowPairingRequest, ctx.device_name);
			}
		}

		private void yesButton_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Yes;
			Close();
		}

		private void noButton_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.No;
			Close();
		}

		private void neverButton_Click(object sender, EventArgs e)
		{
			Settings.Default.RejectOtherDevices = true;
			Settings.Default.Save();
			DialogResult = System.Windows.Forms.DialogResult.No;
			Close();
		}

		private void PairingRequestDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
	}
}
