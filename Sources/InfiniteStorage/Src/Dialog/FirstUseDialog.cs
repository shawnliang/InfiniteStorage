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
		public ProtocolContext FirstConnection { get; private set; }

		static FirstUseDialog instance;

		public Action ApproveFunc { get; set; }

		static FirstUseDialog()
		{
			instance = new FirstUseDialog();
		}


		public static FirstUseDialog Instance
		{
			get
			{
				if (instance.IsDisposed)
					instance = new FirstUseDialog();

				return instance;
			}
		}

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

		public void ShowSetupPage(WebsocketProtocol.ProtocolContext ctx)
		{
			tabControlEx1.PageIndex = 2;
			transferringControl1.WebSocketContext = ctx;
			Show();
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
			if (tabControlEx1.SelectedTab == tabChooseOrganizeMethod)
			{
				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.SingleFolderLocation = storageLocationControl1.StoragePath;
				Settings.Default.OrganizeMethod = (int)organizeSelectionControl1.OrganizeBy;
				Settings.Default.Save();

				if (ApproveFunc != null)
					ApproveFunc();

				tabControlEx1.NextPage();
			}
			else if (tabControlEx1.IsLastPage)
			{
				transferringControl1.StopUpdateUI();
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
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
	}
}
