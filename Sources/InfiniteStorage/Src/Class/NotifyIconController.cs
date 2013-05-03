using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;

namespace InfiniteStorage
{
	class NotifyIconController
	{
		private PreferenceDialog preferenceForm = new PreferenceDialog();
		private NotifyIcon notifyIcon;
		private Dictionary<ProtocolContext, ToolStripItem> deviceStipItems = new Dictionary<ProtocolContext, ToolStripItem>();
		private List<BackupProgressDialog> progressDialogs = new List<BackupProgressDialog>();
		private object cs = new object();

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
			if (evt.ctx.total_files == 0 || evt.ctx.total_files == evt.ctx.recved_files)
			{
				return;
			}


			SynchronizationContextHelper.SendMainSyncContext(() => {
				updateNotifyIconMenu(evt.ctx);
				updateProgressDialog(evt.ctx);
			});
		}

		private void updateNotifyIconMenu(ProtocolContext ctx)
		{
			ToolStripItem itemFound = findMenuItemInNotifyContextMenu(ctx);

			if (itemFound != null)
			{
				itemFound.Text = getOverallProgressText(ctx);

				var nextIdx = notifyIcon.ContextMenuStrip.Items.IndexOf(itemFound) + 1;
				notifyIcon.ContextMenuStrip.Items[nextIdx].Text = getSingleFileText(ctx);
			}
			else
			{
				var item = new ToolStripMenuItem();
				item.Text = getOverallProgressText(ctx);
				item.Tag = ctx;
				item.Enabled = false;
				deviceStipItems.Add(ctx, item);


				var item2 = new ToolStripMenuItem();
				item2.Text = getSingleFileText(ctx);
				item2.Enabled = false;


				notifyIcon.ContextMenuStrip.Items.Insert(2, item);
				notifyIcon.ContextMenuStrip.Items.Insert(3, item2);
				notifyIcon.ShowBalloonTip(3000, Resources.ProductName, string.Format(Resources.BallonText_Transferring, ctx.device_name, ctx.total_files), ToolTipIcon.Info);
			}
		}

		private void updateProgressDialog(ProtocolContext ctx)
		{
			if (!Settings.Default.ShowBackupProgressDialog)
				return;

			lock (cs)
			{
				var dupConnDialog = progressDialogs.Find(x => x.WSCtx.device_id == ctx.device_id);
				if (dupConnDialog != null)
				{
					dupConnDialog.WSCtx = ctx;
				}
				else
				{
					var dialog = new BackupProgressDialog(ctx);
					dialog.FormClosed += onProgressDialogClosed;
					dialog.StartPosition = FormStartPosition.CenterScreen;
					dialog.Show();

					progressDialogs.Add(dialog);
				}
			}
		}

		private void onProgressDialogClosed(object sender, FormClosedEventArgs args)
		{
			lock (cs)
			{
				if (sender is BackupProgressDialog)
				{
					progressDialogs.Remove(sender as BackupProgressDialog);
				}
			}
		}

		private static string getSingleFileText(ProtocolContext ctx)
		{
			return ctx.temp_file != null ?
							string.Format("  - {0} : {1}%", ctx.fileCtx.file_name, 100 * ctx.temp_file.BytesWritten / ctx.fileCtx.file_size) :
							"  - 準備傳送中...";
		}

		private static string getOverallProgressText(ProtocolContext ctx)
		{
			return string.Format("{0}: {1}/{2}", ctx.device_name, ctx.recved_files, ctx.total_files);
		}

		public void refreshNotifyIconContextMenu()
		{
			for (int i = 0; i < notifyIcon.ContextMenuStrip.Items.Count; i++)
			{
				var item = notifyIcon.ContextMenuStrip.Items[i];
				if (item.Tag != null)
				{
					var ctx = item.Tag as WebsocketProtocol.ProtocolContext;

					var overallProgress = getOverallProgressText(ctx);
					if (overallProgress != item.Text)
						item.Text = overallProgress;

					notifyIcon.ContextMenuStrip.Items[i + 1].Text = getSingleFileText(ctx);
				}
			}
		}

		private ToolStripItem findMenuItemInNotifyContextMenu(ProtocolContext targetCtx)
		{
			ToolStripItem itemFound = null;

			foreach (var it in notifyIcon.ContextMenuStrip.Items)
			{
				var stripItem = it as ToolStripItem;
				if (stripItem != null && stripItem.Tag is ProtocolContext)
				{
					var ctx = stripItem.Tag as ProtocolContext;

					if (ctx == targetCtx)
					{
						itemFound = stripItem;
						break;
					}
				}
			}
			return itemFound;
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
			try
			{
				var key = evt.ctx;

				if (deviceStipItems.ContainsKey(key))
				{
					var item = deviceStipItems[key];
					if (notifyIcon.ContextMenuStrip.Items.Contains(item))
					{
						var nextIdx = notifyIcon.ContextMenuStrip.Items.IndexOf(item) + 1;
						var item2 = notifyIcon.ContextMenuStrip.Items[nextIdx];

						notifyIcon.ContextMenuStrip.Items.Remove(item);
						notifyIcon.ContextMenuStrip.Items.Remove(item2);
					}

					deviceStipItems.Remove(key);
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Error in removeDeviceFromNotifyIconMenu", err);
			}
		}

		private void showApproveDialog(WebsocketProtocol.WebsocketEventArgs args)
		{
			var result = MessageBox.Show(string.Format(Resources.AllowPairingRequest, args.ctx.device_name), Resources.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			try
			{
				if (result == DialogResult.No)
				{
					args.ctx.handleDisapprove();
				}
				else
				{

					if (Settings.Default.IsFirstUse)
					{
						if (FirstUseDialog.Instance.Visible)
							FirstUseDialog.Instance.Close();

						FirstUseDialog.Instance.ShowSetupPage(args.ctx);

						Settings.Default.IsFirstUse = false;
						Settings.Default.Save();
					}
					else
					{
						args.ctx.handleApprove();
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Error in showApproveDialog", err);
			}
		}
		
	}
}
