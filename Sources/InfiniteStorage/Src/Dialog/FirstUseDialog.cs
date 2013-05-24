using InfiniteStorage.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class FirstUseDialog : Form
	{
		static FirstUseDialog instance = new FirstUseDialog();


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
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;
		}

		private void FirstUseDialog_Load(object sender, EventArgs e)
		{
			BonjourServiceRegistrator.Instance.Register(true);
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
			if (tabControlEx1.SelectedTab == tabChooseLocation)
			{
				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.SingleFolderLocation = storageLocationControl1.StoragePath;
				Settings.Default.Save();

				// write folder location to registry so that client can get it.
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", storageLocationControl1.StoragePath);

				var bunnyAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName);

				NginxUtility.Instance.PrepareNginxConfig(
					bunnyAppData,
					12888,
					storageLocationControl1.StoragePath
				);

				NginxUtility.Instance.Start(bunnyAppData);

				ThumbnailCreator.Instance.Start();

				try
				{
					transferringControl1.WebSocketContext.handleApprove();
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to send approve msg", err);
				}

				MessageBox.Show(Resources.FirstUse_GoBackToPhoneToStart, Resources.FirstUse_Start, MessageBoxButtons.OK, MessageBoxIcon.Information);

				tabControlEx1.NextPage();
				Close();
			}
			else if (tabControlEx1.IsLastPage)
			{
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


			prevButton.Visible = false;

			nextButton.Text = Resources.FirstUse_Next;
			nextButton.Visible = (selectedTab != tabWelcomeAndWaitConnect);

			if (selectedTab == tabChooseLocation)
				nextButton.Text = Resources.FirstUse_Start;

			if (selectedTab == tabInstalledSuccess)
				nextButton.Text = Resources.FirstUse_ShrinkToSystemTray;


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

		private void FirstUseDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			var curTab = tabControlEx1.SelectedTab;

			if (e.CloseReason == CloseReason.UserClosing &&
				(curTab == tabChooseLocation))
			{
				MessageBox.Show(Resources.FirstUse_NoExitBeforeAcceptComplete, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				e.Cancel = true;
				return;
			}

			transferringControl1.StopUpdateUI();
		}

		private void FirstUseDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			BonjourServiceRegistrator.Instance.Register(false);
		}
	}
}
