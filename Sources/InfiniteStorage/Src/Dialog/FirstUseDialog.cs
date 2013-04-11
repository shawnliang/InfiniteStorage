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

			var userFolder = Environment.GetEnvironmentVariable("UserProfile");
			var myPic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			var myVideo = Path.Combine(userFolder, "Videos");
			var myPodcasts = Path.Combine(userFolder, "Podcasts");

			generalPreferenceControl1.PhotoLocation = Path.Combine(myPic, Resources.ProductName);
			generalPreferenceControl1.VideoLocation = Path.Combine(myVideo, Resources.ProductName);
			generalPreferenceControl1.AudioLocation = Path.Combine(myPodcasts, Resources.ProductName);
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
			if (tabControlEx1.IsLastPage)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;

				Settings.Default.PhotoLocation = generalPreferenceControl1.PhotoLocation;
				Settings.Default.VideoLocation = generalPreferenceControl1.VideoLocation;
				Settings.Default.AudioLocation = generalPreferenceControl1.AudioLocation;
				Settings.Default.OrganizeMethod = (int)generalPreferenceControl1.OrganizeMethod;
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
	}
}
