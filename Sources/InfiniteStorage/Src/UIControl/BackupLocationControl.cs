using InfiniteStorage.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class BackupLocationControl : UserControl
	{
		public BackupLocationControl()
		{
			InitializeComponent();
		}

		private void BackupLocationControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				var settings = Settings.Default;

				LocationType = (LocationType)settings.LocationType;

				var userFolder = Environment.GetEnvironmentVariable("UserProfile");

				SingleFolderLocation = settings.SingleFolderLocation;
				if (string.IsNullOrEmpty(SingleFolderLocation))
				{
					SingleFolderLocation = Path.Combine(userFolder, Resources.ProductName);
				}

				CustomPhotoLocation = settings.CustomPhotoLocation;
				if (string.IsNullOrEmpty(CustomPhotoLocation))
				{
					CustomPhotoLocation = Path.Combine(userFolder, @"Pictures\" + Resources.ProductName);
				}

				CustomVideoLocation = settings.CustomVideoLocation;
				if (string.IsNullOrEmpty(CustomVideoLocation))
				{
					CustomVideoLocation = Path.Combine(userFolder, @"Videos\" + Resources.ProductName);
				}

				CustomAudioLocation = settings.CustomAudioLocation;
				if (string.IsNullOrEmpty(CustomAudioLocation))
				{
					CustomAudioLocation = Path.Combine(userFolder, @"Podcasts\" + Resources.ProductName);
				}
			}
		}

		private void radioCustom_CheckedChanged(object sender, EventArgs e)
		{
			syncRightPanel();
		}

		private void syncRightPanel()
		{
			if (radioCustom.Checked)
				tabControlEx1.SelectedTab = tabCustom;
			else if (radioSingleFolder.Checked)
				tabControlEx1.SelectedTab = tabSingleFolder;
			else if (radioMediaLibrary.Checked)
				tabControlEx1.SelectedTab = tabMediaLibrary;
			else
				throw new NotImplementedException();
		}

		public LocationType LocationType
		{
			get
			{
				if (radioSingleFolder.Checked)
					return InfiniteStorage.LocationType.SingleFolder;
				else if (radioMediaLibrary.Checked)
					return InfiniteStorage.LocationType.MediaLibrary;
				else
					return InfiniteStorage.LocationType.Custom;
			}

			set
			{
				switch (value)
				{
					case InfiniteStorage.LocationType.SingleFolder:
						radioSingleFolder.Checked = true;
						break;

					case InfiniteStorage.LocationType.MediaLibrary:
						radioMediaLibrary.Checked = true;
						break;

					case InfiniteStorage.LocationType.Custom:
						radioCustom.Checked = true;
						break;

					default:
						throw new NotImplementedException();
				}

				syncRightPanel();
			}
		}


		public string SingleFolderLocation
		{
			get { return boxSingleFolder.Text; }
			set { boxSingleFolder.Text = value; }
		}

		public string CustomPhotoLocation
		{
			get { return lblPhotoLocation.Text; }
			set { lblPhotoLocation.Text = value; }
		}

		public string CustomVideoLocation
		{
			get { return lblVideoLocation.Text; }
			set { lblVideoLocation.Text = value; }
		}

		public string CustomAudioLocation
		{
			get { return lblAudioLocation.Text; }
			set { lblAudioLocation.Text = value; }
		}

		private void changeCustomLocation_Click(object sender, EventArgs e)
		{
			if (sender == changePhotoLocation)
				showBrowserFolderDialogForLabel(lblPhotoLocation);
			else if (sender == changeVideoLocation)
				showBrowserFolderDialogForLabel(lblVideoLocation);
			else if (sender == changeAudioLocation)
				showBrowserFolderDialogForLabel(lblAudioLocation);
			else
				throw new NotImplementedException();
		}

		private void changeSingleFolderButton_Click(object sender, EventArgs e)
		{
			showBrowserFolderDialogForLabel(boxSingleFolder);
		}

		private void showBrowserFolderDialogForLabel(Control lbl)
		{
			var dialog = new FolderBrowserDialog();
			if (!string.IsNullOrEmpty(lbl.Text))
				dialog.SelectedPath = Path.GetDirectoryName(lbl.Text);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				lbl.Text = Path.Combine(dialog.SelectedPath, Resources.ProductName);
			}
		}
	}


	public enum LocationType
	{
		SingleFolder = 0,
		MediaLibrary,
		Custom,
	}
}
