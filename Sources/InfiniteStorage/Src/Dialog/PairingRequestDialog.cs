using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using System;
using System.Windows.Forms;

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
			Icon = Resources.ProductIcon;
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
