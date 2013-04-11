using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Bonjour;
using WebSocketSharp.Server;
using log4net;
using InfiniteStorage.Properties;
using System.IO;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static NotifyIconController m_notifyIconController;

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

			if (showFirstUseWizard() != DialogResult.OK)
				return;

			initNotifyIcon();

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
			m_notifyIconController = new NotifyIconController();

			m_notifyIcon.ContextMenu = new ContextMenu(
				new MenuItem[] {
					new MenuItem(Resources.TrayMenuItem_OpenBackupFolder, new MenuItem[] {
						new MenuItem("Photos", m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked),
						new MenuItem("Videos", m_notifyIconController.OnOpenVideoBackupFolderMenuItemClicked),
						new MenuItem("Audios", m_notifyIconController.OnOpenAudioBackupFolderMenuItemClicked)
					}),
					new MenuItem(Resources.TrayMenuItem_Preferences, m_notifyIconController.OnPreferencesMenuItemClicked),
					new MenuItem(Resources.TrayMenuItem_GettingStarted, m_notifyIconController.OnGettingStartedMenuItemClicked),
					new MenuItem("-"),
					new MenuItem(Resources.TrayMenuItem_Quit, m_notifyIconController.OnQuitMenuItemClicked),
				});
			m_notifyIcon.Visible = true;

			m_notifyIcon.ShowBalloonTip(3000, Resources.ProductName, "超屌備份在此為您服務", ToolTipIcon.None);
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
