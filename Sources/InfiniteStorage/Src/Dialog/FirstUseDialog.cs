using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using System.IO;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	public partial class FirstUseDialog : Form
	{
		private ProtocolContext pendingCtx;

		public FirstUseDialog()
		{
			InitializeComponent();
			Icon = Resources.product_icon;
			Text = Resources.ProductName;
		}

		private void FirstUseDialog_Load(object sender, EventArgs e)
		{
			instruction1.Text = string.Format(Resources.FirstUse_Instruction1, BonjourService.ServiceName);

			UpdateUI();
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
			if (tabControlEx1.SelectedTab == tabChooseOrganizeMethod)
			{
				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.SingleFolderLocation = storageLocationControl1.StoragePath;
				Settings.Default.OrganizeMethod = (int)organizeSelectionControl1.OrganizeBy;
				Settings.Default.Save();

				if (pendingCtx != null)
					pendingCtx.handleApprove();

				tabControlEx1.NextPage();
			}
			else if (tabControlEx1.IsLastPage)
			{
				transferringControl1.StopUpdateUI();
				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			else
				tabControlEx1.NextPage();
		}

		private void UpdateUI()
		{
			var selectedTab = tabControlEx1.SelectedTab;

			if (selectedTab == null)
				return;

			this.Text = selectedTab.Text;


			prevButton.Visible = selectedTab == tabChooseOrganizeMethod;

			nextButton.Text = "Next";

			if (selectedTab == tabChooseOrganizeMethod)
				nextButton.Text = "開始備份";

			if (selectedTab == tabInstalledSuccess)
				nextButton.Text = "縮到狀態列";


			nextButton.Enabled = (selectedTab != tabWelcomeAndWaitConnect);
		}

		private void tabControlEx1_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void prevButton_Click(object sender, EventArgs e)
		{
			tabControlEx1.PreviousPage();
		}

		private void getItOnGooglePlay_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not imp yet");
		}

		private void getItOnAppStore_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not imp yet");
		}

		public void OnPairingRequesting(object sender, WebsocketEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(() =>
				{
					OnPairingRequesting(sender, e);
				}));
			}
			else
			{
				if (tabControlEx1.SelectedTab == tabWelcomeAndWaitConnect)
				{
					if (MessageBox.Show(string.Format(Resources.AllowPairingRequest, e.ctx.device_name), Resources.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						pendingCtx = e.ctx;
						transferringControl1.WebSocketContext = e.ctx;
						tabControlEx1.NextPage();
					}
					else
					{
						try
						{
							e.ctx.handleDisapprove();
						}
						catch (Exception err)
						{
							log4net.LogManager.GetLogger(GetType()).Warn("Unable to disapprove pairing request from " + e.ctx.device_name, err);
						}
					}
				}
			}
		}
	}
}
