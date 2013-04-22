﻿using System;
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
using System.Drawing;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static NotifyIconController m_notifyIconController;
		static Timer m_NotifyTimer;
		static System.Timers.Timer m_BackupStatusTimer;

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

			if (string.IsNullOrEmpty(Settings.Default.SingleFolderLocation))
			{
				if (showFirstUseWizard() != DialogResult.OK)
					return;
			}

			initNotifyIcon();
			initConnectedDeviceCollection();
			initBackupStatusTimer();


			SynchronizationContextHelper.SetMainSyncContext();

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

		private static void initBackupStatusTimer()
		{
			m_BackupStatusTimer = new System.Timers.Timer(10 * 1000);
			m_BackupStatusTimer.Elapsed += (s, e) =>
			{
				m_BackupStatusTimer.Enabled = false;
				var updator = new ConnectionStatusUpdator();
				updator.UpdateStatusToPeers();
				m_BackupStatusTimer.Enabled = true;
			};
			m_BackupStatusTimer.Start();
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

			var openStorageItem = m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_OpenBackupFolder, null, m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked);
			openStorageItem.Font = new Font(openStorageItem.Font, FontStyle.Bold);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Preferences, null, m_notifyIconController.OnPreferencesMenuItemClicked);

			//m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_GettingStarted, null, m_notifyIconController.OnGettingStartedMenuItemClicked);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Quit, null, m_notifyIconController.OnQuitMenuItemClicked);

			m_notifyIcon.Visible = true;

			m_notifyIcon.ShowBalloonTip(3000, Resources.ProductName, "超屌備份在此為您服務", ToolTipIcon.None);

			m_notifyIcon.DoubleClick += m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked;

			InfiniteStorageWebSocketService.DeviceAccepted += m_notifyIconController.OnDeviceConnected;
			InfiniteStorageWebSocketService.DeviceDisconnected += m_notifyIconController.OnDeviceDisconnected;
			InfiniteStorageWebSocketService.PairingRequesting += m_notifyIconController.OnDevicePairingRequesting;
		}

		private static void initConnectedDeviceCollection()
		{
			InfiniteStorageWebSocketService.DeviceAccepted += (s, e) => { ConnectedClientCollection.Instance.Add(e.ctx); };
			InfiniteStorageWebSocketService.DeviceDisconnected += (s, e) => { ConnectedClientCollection.Instance.Remove(e.ctx); };
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
