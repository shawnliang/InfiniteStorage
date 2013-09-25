using InfiniteStorage.Properties;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class MigratingDataDialog : Form
	{
		bool m_closeByApp;

		public MigratingDataDialog()
		{
			InitializeComponent();

			Icon = Resources.ProductIcon;
		}

		public void CloseByApp()
		{
			m_closeByApp = true;

			Close();
		}

		private void MigratingDataDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && !m_closeByApp)
			{
				e.Cancel = true;
			}
		}
	}
}
