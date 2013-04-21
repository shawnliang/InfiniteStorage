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
		private bool actionTaken;

		public PairingRequestDialog(ProtocolContext ctx)
		{
			this.ctx = ctx;
			InitializeComponent();
			Text = Resources.ProductName;
			Icon = Resources.product_icon;

		}

		private void PairingRequestDialog_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				questionLabel.Text = string.Format("允許 {0} 備份檔案到這台電腦?", ctx.device_name);
			}
		}

		private void yesButton_Click(object sender, EventArgs e)
		{
			takeActionAndLogError(ctx.handleApprove);
			Close();
		}

		private void noButton_Click(object sender, EventArgs e)
		{
			takeActionAndLogError(ctx.handleDisapprove);
			Close();

		}

		private void neverButton_Click(object sender, EventArgs e)
		{
			takeActionAndLogError(ctx.handleDisapprove);
			Settings.Default.RejectOtherDevices = true;
			Settings.Default.Save();
			Close();
		}

		private void PairingRequestDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!actionTaken)
			{
				takeActionAndLogError(ctx.handleDisapprove);
			}
		}

		private void takeActionAndLogError(Action act)
		{
			try
			{
				act();
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Uable to approve/disapprove for " + ctx.device_name, e);
			}

			actionTaken = true;
		}
	}
}
