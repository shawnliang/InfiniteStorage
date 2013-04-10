using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InfiniteStorage
{
	class NotifyIconController
	{
		private Form1 preferenceForm = new Form1();

		public void OnOpenBackupFolderMenuItemClicked(object sender, EventArgs arg)
		{

		}

		public void OnPreferencesMenuItemClicked(object sender, EventArgs arg)
		{
			if (preferenceForm.IsDisposed)
				preferenceForm = new Form1();

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
