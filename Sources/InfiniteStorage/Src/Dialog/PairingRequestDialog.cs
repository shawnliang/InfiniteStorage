using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using System;
using System.Threading;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class PairingRequestDialog : Form
	{
		static int concurrentCount;

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
			Interlocked.Decrement(ref concurrentCount);
		}

		private void PairingRequestDialog_Shown(object sender, EventArgs e)
		{
			var count = Interlocked.Increment(ref concurrentCount);

			Location = new System.Drawing.Point(Location.X + count * 10, Location.Y + count * 10);
		}
	}
}
