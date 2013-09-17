using InfiniteStorage.Properties;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class MigratingDataDialog : Form
	{
		bool closeByApp;

		public MigratingDataDialog()
		{
			InitializeComponent();

			Icon = Resources.ProductIcon;
		}

		public void CloseByApp()
		{
			closeByApp = true;

			Close();
		}

		private void MigratingDataDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && !closeByApp)
			{
				e.Cancel = true;
			}
		}
	}
}
