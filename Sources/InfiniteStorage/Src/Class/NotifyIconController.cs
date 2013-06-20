using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace InfiniteStorage
{
	class NotifyIconController
	{
		private PreferenceDialog preferenceForm;
		private NotifyIcon notifyIcon;
		private Dictionary<ProtocolContext, ToolStripItem> deviceStipItems = new Dictionary<ProtocolContext, ToolStripItem>();
		private StationServer stationServer;

		public NotifyIconController(NotifyIcon notifyIcon, StationServer station)
		{
			this.notifyIcon = notifyIcon;
			this.stationServer = station;
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

		public void OnOpenUIMenuItemCliecked(object sender, EventArgs arg)
		{
			ImportUIPresenter.Instance.StartViewer();
		}

		public void OnFileReceiving(object sender, WebsocketEventArgs arg)
		{
			if (!arg.ctx.BackToPhoneDialogClosed)
			{
				BackToPhoneDialog.CloseOpenedWindow(arg.ctx);
				arg.ctx.BackToPhoneDialogClosed = true;
			}
		}

		private static void openFolderInExplorer(string folder)
		{
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			Process.Start(folder);
		}

		public void OnPreferencesMenuItemClicked(object sender, EventArgs arg)
		{
			if (preferenceForm == null || preferenceForm.IsDisposed)
				preferenceForm = new PreferenceDialog(stationServer);

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
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				showNotifyIconMenu(evt.ctx);
			});
		}

		public void OnTotalCountUpdated(object sender, WebsocketEventArgs evt)
		{
			SynchronizationContextHelper.SendMainSyncContext(() =>
			{
				updateNotifyIconMenu(evt.ctx);
			});
		}

		private void updateNotifyIconMenu(ProtocolContext ctx)
		{
			ToolStripItem itemFound = findItemFromSameCtx(ctx);

			if (itemFound != null)
			{
				itemFound.Text = getOverallProgressText(ctx);

				var nextIdx = notifyIcon.ContextMenuStrip.Items.IndexOf(itemFound) + 1;

				notifyIcon.ContextMenuStrip.Items[nextIdx].Text = getSingleFileText(ctx);
			}
		}

		private void showNotifyIconMenu(ProtocolContext ctx)
		{

			int index;
			var saveDev = findItemFromSameDevice(ctx, out index);
			if (saveDev != null)
			{
				notifyIcon.ContextMenuStrip.Items.RemoveAt(index + 1);
				notifyIcon.ContextMenuStrip.Items.Remove(saveDev);
			}


			ToolStripItem itemFound = findItemFromSameCtx(ctx);

			if (itemFound == null)
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
				//notifyIcon.ShowBalloonTip(3000, Resources.ProductName, string.Format(Resources.BallonText_Transferring, ctx.device_name, ctx.total_count), ToolTipIcon.Info);
			}

		}

		private static string getSingleFileText(ProtocolContext ctx)
		{
			if (ctx.NoMoreToTransfer())
			{
				return Resources.MenuItem_BackupComplete;
			}

			return ctx.temp_file != null ?
							string.Format("  - {0} : {1}%", ctx.fileCtx.file_name, 100 * ctx.temp_file.BytesWritten / ctx.fileCtx.file_size) :
							Resources.MenuItem_Preparing;
		}

		private static string getOverallProgressText(ProtocolContext ctx)
		{
			return ctx.total_count > 0 ? string.Format("{0}: {1}/{2}", ctx.device_name, ctx.backup_count, ctx.total_count) : ctx.device_name;
		}

		public void refreshNotifyIconContextMenu()
		{
			try
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
			catch(Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to update tray icon status", err);
			}
		}

		private ToolStripItem findItemFromSameDevice(ProtocolContext targetCtx, out int index)
		{
			index = -1;
			ToolStripItem itemFound = null;

			//foreach (var it in notifyIcon.ContextMenuStrip.Items)
			for (int i = 0; i < notifyIcon.ContextMenuStrip.Items.Count; i++)
			{
				var stripItem = notifyIcon.ContextMenuStrip.Items[i] as ToolStripItem;
				if (stripItem != null && stripItem.Tag is ProtocolContext)
				{
					var ctx = stripItem.Tag as ProtocolContext;

					if (ctx.device_id == targetCtx.device_id)
					{
						itemFound = stripItem;
						index = i;
						break;
					}
				}
			}
			return itemFound;
		}

		private ToolStripItem findItemFromSameCtx(ProtocolContext targetCtx)
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
			try
			{
				if (!BonjourServiceRegistrator.Instance.IsAccepting)
				{
					args.ctx.handleDisapprove();
					return;
				}
				else
				{

					var pairingDialog = new PairingRequestDialog(args.ctx);
					if (pairingDialog.ShowDialog() != DialogResult.Yes)
					{
						var t = new System.Threading.Tasks.Task(() =>
						{
							try
							{
								args.ctx.handleDisapprove();
							}
							catch (Exception err)
							{
								log4net.LogManager.GetLogger(GetType()).Warn("disapprove error", err);
							}
						});
						t.Start();

						return;
					}

					args.ctx.handleApprove();

					if (WaitForPairingDialog.Instance.Visible && PairingRequestDialog.CurrentOpeningCount == 0)
						WaitForPairingDialog.Instance.Close();

					if (Settings.Default.IsFirstUse)
					{
						Settings.Default.IsFirstUse = false;
						Settings.Default.Save();
					}

					// Show Hint to press "start on PC"
					var backToPhoneDialog = new BackToPhoneDialog() { Ctx = args.ctx };
					backToPhoneDialog.Show();
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("showApproveDialog error", err);
			}
		}


		public void OnAddingNewSources(object sender, EventArgs e)
		{
			WaitForPairingDialog.Instance.Show();
			WaitForPairingDialog.Instance.Activate();
			WaitForPairingDialog.Instance.BringToFront();
		}
	}
}
