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

namespace InfiniteStorage
{
	public partial class FirstUseDialog : Form
	{
		public FirstUseDialog()
		{
			InitializeComponent();
			Icon = Resources.product_icon;
			Text = Resources.ProductName;
		}

		private void FirstUseDialog_Load(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
			if (tabControlEx1.IsLastPage)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;

				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.SingleFolderLocation = storageLocationControl1.StoragePath;
				Settings.Default.OrganizeMethod = (int)organizeSelectionControl1.OrganizeBy;
				Settings.Default.Save();
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


			prevButton.Visible = selectedTab != tabControlEx1.TabPages[0];

			nextButton.Text = (selectedTab == tabControlEx1.TabPages[tabControlEx1.TabCount - 1]) ?
				"Start" : "Next";
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
	}
}
