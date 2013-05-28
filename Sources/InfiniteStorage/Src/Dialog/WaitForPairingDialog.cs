using InfiniteStorage.Properties;
using System;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class WaitForPairingDialog : Form
	{
		private static WaitForPairingDialog instance;

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
		}

		private void WaitForPairingDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			BonjourServiceRegistrator.Instance.Register(false);
		}
	}
}
