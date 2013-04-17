using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;

namespace InfiniteStorage
{
	class NotifyIconController
	{
		private PreferenceDialog preferenceForm = new PreferenceDialog();
		private NotifyIcon notifyIcon;
		private Dictionary<ProtocolContext, ToolStripItem> deviceStipItems = new Dictionary<ProtocolContext, ToolStripItem>();
		private List<PairingRequestDialog> approveDialogs = new List<PairingRequestDialog>();
		

		public NotifyIconController(NotifyIcon notifyIcon)
		{
			this.notifyIcon = notifyIcon;
		}

		public void OnOpenPhotoBackupFolderMenuItemClicked(object sender, EventArgs arg)
		{
			openFolderInExplorer(MyFileFolder.Photo);
		}

		public void OnOpenVideoBackupFolderMenuItemClicked(object sender, EventArgs arg)
		{
			openFolderInExplorer(MyFileFolder.Video);
		}

		public void OnOpenAudioBackupFolderMenuItemClicked(object sender, EventArgs arg)
		{
			openFolderInExplorer(MyFileFolder.Audio);
		}

		private static void openFolderInExplorer(string folder)
		{
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			Process.Start(folder);
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


		public void OnDeviceConnected(object sender, WebsocketEventArgs evt)
		{
			if (notifyIcon.ContextMenuStrip.InvokeRequired)
			{
				notifyIcon.ContextMenuStrip.Invoke(
					new MethodInvoker(() =>
					{
						OnDeviceConnected(sender, evt);
					}));
			}
			else
			{
				var item = new ToolStripMenuItem();
				item.Text = string.Format("{0}: {1}/{2}", evt.ctx.device_name, evt.ctx.recved_files, evt.ctx.total_files);
				item.Tag = evt.ctx;
				item.Enabled = false;
				deviceStipItems.Add(evt.ctx, item);

				notifyIcon.ContextMenuStrip.Items.Insert(2, item);
				notifyIcon.ShowBalloonTip(3000, Resources.ProductName, string.Format("{0} 正在備份 {1} 個擋案", evt.ctx.device_name, evt.ctx.total_files), ToolTipIcon.Info);
			}
		}

		public void OnDeviceDisconnected(object sender, WebsocketEventArgs evt)
		{
			SynchronizationContextHelper.SendMainSyncContext(() => { removeDeviceFromNotifyIconMenu(evt); });
			
		}

		public void OnDevicePairingRequesting(object sender, WebsocketProtocol.WebsocketEventArgs args)
		{
			SynchronizationContextHelper.SendMainSyncContext(() => { showApproveDialog(args); });
		}

		private void removeDeviceFromNotifyIconMenu(WebsocketEventArgs evt)
		{
			var key = evt.ctx;

			if (deviceStipItems.ContainsKey(key))
			{
				var item = deviceStipItems[key];
				notifyIcon.ContextMenuStrip.Items.Remove(item);

				deviceStipItems.Remove(key);
			}
		}

		private void showApproveDialog(WebsocketProtocol.WebsocketEventArgs args)
		{
			var dialog = new PairingRequestDialog(args.ctx);

			dialog.StartPosition = FormStartPosition.Manual;

			var paddingHeight = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height + 10;

			var workingArea = Screen.PrimaryScreen.Bounds;
			dialog.Left = workingArea.Left + workingArea.Width - dialog.Width;
			dialog.Top = workingArea.Top + workingArea.Height - dialog.Height - paddingHeight;
			dialog.StartPosition = FormStartPosition.Manual;
			dialog.TopMost = true;
			dialog.Show();
		}
	}
}
