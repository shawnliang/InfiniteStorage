using System.Windows.Forms;
using System;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class GeneralPreferenceControl : UserControl
	{
		public event EventHandler SettingsChanged;

		public GeneralPreferenceControl()
		{
			InitializeComponent();
		}

		private void libraryName_TextChanged(object sender, EventArgs e)
		{
			var handler = SettingsChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public string LibraryName
		{
			get { return libraryName.Text; }
		}

		private void GeneralPreferenceControl_Load(object sender, EventArgs e)
		{
			libraryName.Text = Settings.Default.LibraryName;
			storageLocationControl1.Enabled = true;
		}
	}
}
