using InfiniteStorage.Properties;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace InfiniteStorage
{
	public partial class WaitForPairingDialog : Form
	{
		private static WaitForPairingDialog instance;
		private BackgroundWorker bgworker = new BackgroundWorker();
		private Timer timer = new Timer();

		public WaitForPairingDialog()
		{
			InitializeComponent();
		}

		public static WaitForPairingDialog Instance
		{
			get
			{
				if (instance == null || instance.IsDisposed)
					instance = new WaitForPairingDialog();

				return instance;
			}
		}

		private void WaitForPairingDialog_Load(object sender, EventArgs e)
		{
			Icon = Resources.ProductIcon;
			Text = Resources.ProductName;
		}

		private void WaitForPairingDialog_Shown(object sender, EventArgs e)
		{
			BonjourServiceRegistrator.Instance.Register(true);

			timer.Interval = 30 * 1000;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;

			BonjourServiceRegistrator.Instance.Register(false);

			if (MessageBox.Show(Resources.WaitForPair_SearchAgainMsg, Resources.WaitForPair_SearchAgainTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
			{
				BonjourServiceRegistrator.Instance.Register(true);
				timer.Enabled = true;
			}
			else
			{
				Close();
			}
		}

		private void WaitForPairingDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			timer.Stop();
			BonjourServiceRegistrator.Instance.Register(false);
		}
	}
}
