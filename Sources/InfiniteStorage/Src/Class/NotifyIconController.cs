using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace InfiniteStorage
{
	class NotifyIconController
	{
		private PreferenceDialog preferenceForm = new PreferenceDialog();

		public void OnOpenBackupFolderMenuItemClicked(object sender, EventArgs arg)
		{
			// TODO: use non-hardcode data
			var userFolder = Environment.GetEnvironmentVariable("USERPROFILE");
			var appFolder = Path.Combine(userFolder, "InfiniteStorage");
			var tempFolder = Path.Combine(appFolder, "temp");
			var deviceFolder = Path.Combine(appFolder, "samsung gt-9300");


			if (!Directory.Exists(deviceFolder))
				Directory.CreateDirectory(deviceFolder);

			Process.Start(deviceFolder);
		}

		public void OnPreferencesMenuItemClicked(object sender, EventArgs arg)
		{
			if (preferenceForm.IsDisposed)
				preferenceForm = new PreferenceDialog();

			preferenceForm.Show();
			preferenceForm.Activate();
		}

		public void OnGettingStartedMenuItemClicked(object sender, EventArgs arg)
		{
		}

		public void OnQuitMenuItemClicked(object sender, EventArgs arg)
		{
			Application.Exit();
		}
	}
}
