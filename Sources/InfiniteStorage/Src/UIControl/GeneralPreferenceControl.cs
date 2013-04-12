using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using InfiniteStorage.Properties;

namespace InfiniteStorage
{
	public partial class GeneralPreferenceControl : UserControl
	{
		public GeneralPreferenceControl()
		{
			InitializeComponent();
		}

		private void GeneralPreferenceControl_Load(object sender, EventArgs e)
		{
			lblComputerName.Text = Environment.MachineName;
			
			if (!DesignMode)
				OrganizeMethod = (OrganizeMethod)Settings.Default.OrganizeMethod;
		}


		public LocationType LocationType
		{
			get { return backupLocationControl.LocationType; }
		}

		public string SingleFolderLocation
		{
			get { return backupLocationControl.SingleFolderLocation; }
		}

		public string CustomPhotoLocation
		{
			get { return backupLocationControl.CustomPhotoLocation; }
		}

		public string CustomVideoLocation
		{
			get { return backupLocationControl.CustomVideoLocation; }
		}

		public string CustomAudioLocation
		{
			get { return backupLocationControl.CustomAudioLocation; }
		}

		public OrganizeMethod OrganizeMethod
		{
			get {
				if (radioY.Checked)
					return InfiniteStorage.OrganizeMethod.Year;
				else if (radioYM.Checked)
					return InfiniteStorage.OrganizeMethod.YearMonth;
				else
					return InfiniteStorage.OrganizeMethod.YearMonthDay;
			}

			set
			{
				var method = value;

				switch (method)
				{
					case InfiniteStorage.OrganizeMethod.Year:
						radioY.Checked = true;
						break;
					case InfiniteStorage.OrganizeMethod.YearMonth:
						radioYM.Checked = true;
						break;
					case InfiniteStorage.OrganizeMethod.YearMonthDay:
						radioYMD.Checked = true;
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}
	}
}
