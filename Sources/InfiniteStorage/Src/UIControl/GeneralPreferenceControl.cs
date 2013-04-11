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
		}


		public string PhotoLocation
		{
			get { return boxPhotoLocation.Text; }
			set { boxPhotoLocation.Text = value; }
		}
		
		public string VideoLocation
		{
			get { return boxVideoLocation.Text; }
			set { boxVideoLocation.Text = value; }
		}

		public string AudioLocation
		{
			get { return boxAudioLocation.Text; }
			set { boxAudioLocation.Text = value; }
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

		private void changeLocation_Click(object sender, EventArgs e)
		{
			TextBox box;

			if (sender == changePhotoLocation)
				box = boxPhotoLocation;
			else if (sender == changeVedioLocation)
				box = boxVideoLocation;
			else if (sender == changeAudioLocation)
				box = boxAudioLocation;
			else
				throw new NotImplementedException();

			changeLocation(box);
		}

		private static void changeLocation(TextBox box)
		{
			var dialog = new FolderBrowserDialog();
			if (!string.IsNullOrEmpty(box.Text))
				dialog.SelectedPath = Path.GetDirectoryName(box.Text);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				var newLocation = Path.Combine(dialog.SelectedPath, Resources.ProductName);
				box.Text = newLocation;
			}
		}
	}
}
