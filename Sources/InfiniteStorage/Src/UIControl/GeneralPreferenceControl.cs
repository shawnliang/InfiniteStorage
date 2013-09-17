#region

using System;
using System.Windows.Forms;
using InfiniteStorage.Properties;

#endregion

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
			if (!DesignMode)
			{
				libraryName.Text = Settings.Default.LibraryName;
				storageLocationControl1.Enabled = true;
			}
		}

		public StationServer Station
		{
			get { return storageLocationControl1.Station; }
			set { storageLocationControl1.Station = value; }
		}
	}
}