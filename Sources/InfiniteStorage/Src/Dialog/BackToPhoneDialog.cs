using InfiniteStorage.Properties;
using System;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class BackToPhoneDialog : Form
	{
		private static BackToPhoneDialog instance;

		public static BackToPhoneDialog Instance
		{
			get
			{
				if (instance == null || instance.IsDisposed)
					instance = new BackToPhoneDialog();

				return instance;
			}
		}

		public BackToPhoneDialog()
		{
			InitializeComponent();
		}

		private void BackToPhoneDialog_Load(object sender, EventArgs e)
		{
			this.Icon = Resources.ProductIcon;
			this.Text = Resources.ProductName;
		}

	}
}
