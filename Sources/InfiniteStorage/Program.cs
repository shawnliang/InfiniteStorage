using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bonjour;
using WebSocketSharp.Server;
using log4net;
using InfiniteStorage.Properties;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Data.SQLite;
using InfiniteStorage.Model;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static NotifyIconController m_notifyIconController;
		static Timer m_NotifyTimer;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			Log4netConfigure.InitLog4net();
			log4net.LogManager.GetLogger("main").Debug("==== program started ====");

			MyDbContext.InitialzeDatabaseSchema();

			if (showFirstUseWizard() != DialogResult.OK)
				return;

			initNotifyIcon();


			m_NotifyTimer = new Timer();
			m_NotifyTimer.Tick += new EventHandler(m_NotifyTimer_Tick);
			m_NotifyTimer.Interval = 1000;
			m_NotifyTimer.Start();


			var port = (ushort)13895;
			WebSocketServer<InfiniteStorageWebSocketService> ws_server = null;
			try
			{
				m_bonjourService = new BonjourService();
				m_bonjourService.Error += new EventHandler<BonjourErrorEventArgs>(m_bonjourService_Error);

				if (string.IsNullOrEmpty(Settings.Default.ServerId))
				{
					Settings.Default.ServerId = Guid.NewGuid().ToString();
					Settings.Default.Save();
				}
				m_bonjourService.Register(port, Settings.Default.ServerId);

				var url = string.Format("ws://0.0.0.0:{0}/", port);
				ws_server = new WebSocketSharp.Server.WebSocketServer<InfiniteStorageWebSocketService>(url);
				ws_server.Start();


			}
			catch
			{
				MessageBox.Show("Bonjour service is not available", "Error");
				return;
			}

			Application.Run();

			m_bonjourService.Unregister();
			ws_server.Stop();
		}

		static void m_NotifyTimer_Tick(object sender, EventArgs e)
		{
			refreshNotifyIconContextMenu();
		}

		private static void refreshNotifyIconContextMenu()
		{
			for (int i = 0; i < m_notifyIcon.ContextMenuStrip.Items.Count; i++)
			{
				var item = m_notifyIcon.ContextMenuStrip.Items[i];
				if (item.Tag != null)
				{
					var ctx = item.Tag as WebsocketProtocol.ProtocolContext;

					var newText = string.Format("{0}: {1}/{2}", ctx.device_name, ctx.recved_files, ctx.total_files);
					if (newText != item.Text)
						item.Text = newText;
				}
			}
		}

		private static DialogResult showFirstUseWizard()
		{
			var firstUseDialog = new FirstUseDialog();
			return firstUseDialog.ShowDialog();
		}

		private static void initNotifyIcon()
		{
			m_notifyIcon = new NotifyIcon();
			m_notifyIcon.Text = Resources.ProductName;
			m_notifyIcon.Icon = Resources.product_icon;
			m_notifyIconController = new NotifyIconController(m_notifyIcon);

			m_notifyIcon.ContextMenuStrip = new ContextMenuStrip();

			var openFolderItem = new ToolStripMenuItem(Resources.TrayMenuItem_OpenBackupFolder, null, new ToolStripItem[]{
				new ToolStripMenuItem("Photo", null, m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked),
				new ToolStripMenuItem("Videos", null, m_notifyIconController.OnOpenVideoBackupFolderMenuItemClicked),
				new ToolStripMenuItem("Audios", null, m_notifyIconController.OnOpenAudioBackupFolderMenuItemClicked),
			});
			m_notifyIcon.ContextMenuStrip.Items.Add(openFolderItem);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Preferences, null, m_notifyIconController.OnPreferencesMenuItemClicked);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_GettingStarted, null, m_notifyIconController.OnGettingStartedMenuItemClicked);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Quit, null, m_notifyIconController.OnQuitMenuItemClicked);

			m_notifyIcon.Visible = true;

			m_notifyIcon.ShowBalloonTip(3000, Resources.ProductName, "超屌備份在此為您服務", ToolTipIcon.None);

			InfiniteStorageWebSocketService.DeviceAccepted += m_notifyIconController.OnDeviceConnected;
			InfiniteStorageWebSocketService.DeviceDisconnected += m_notifyIconController.OnDeviceDisconnected;
			InfiniteStorageWebSocketService.PairingRequesting += m_notifyIconController.OnDevicePairingRequesting;
		}

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			if (m_notifyIcon != null)
				m_notifyIcon.Dispose();
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
			log4net.LogManager.GetLogger("main").Warn("Bonjour DNS operation error: " + e.error.ToString());
		}
	}
}
