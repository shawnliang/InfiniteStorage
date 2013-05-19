using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using InfiniteStorage.Properties;
using InfiniteStorage.REST;
using InfiniteStorage.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Wammer.Station;
using WebSocketSharp.Server;

namespace InfiniteStorage
{
	static class Program
	{
		static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static NotifyIconController m_notifyIconController;
		static Timer m_NotifyTimer;
		static System.Timers.Timer m_BackupStatusTimer;
		static System.Timers.Timer m_ReRegBonjourTimer;
		static WebSocketServer<InfiniteStorageWebSocketService> backup_server;
		static WebSocketServer<NotifyWebSocketService> notify_server;
		static HttpServer rest_server;
		static Notifier notifier;

		private static System.Threading.Mutex m_InstanceMutex { get; set; }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Boolean bCreatedNew;

			m_InstanceMutex = new System.Threading.Mutex(false, Application.ProductName + Environment.UserName, out bCreatedNew);

			if (!bCreatedNew)
			{
				invokeAnotherRunningProcess();
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
			Application.ThreadException += NBug.Handler.ThreadException;

			Log4netConfigure.InitLog4net();
			log4net.LogManager.GetLogger("main").Debug("==== program started ====");

			if (!Settings.Default.IsUpgraded)
			{
				Settings.Default.Upgrade();
				Settings.Default.IsUpgraded = true;
				Settings.Default.Save();
			}

			if (string.IsNullOrEmpty(Settings.Default.ServerId))
			{
				Settings.Default.ServerId = generateSameServerIdForSameUserOnSamePC();
				Settings.Default.SingleFolderLocation = MediaLibrary.UserFolder;
				Settings.Default.OrganizeMethod = (int)OrganizeMethod.YearMonth;
				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.Save();
			}


			SynchronizationContextHelper.SetMainSyncContext();
			DBInitializer.InitialzeDatabaseSchema();
			SeqNum.InitFromDB();

			initNotifyIcon();

			m_NotifyTimer = new Timer();
			m_NotifyTimer.Tick += new EventHandler(m_NotifyTimer_Tick);
			m_NotifyTimer.Interval = 200;
			m_NotifyTimer.Start();

			initConnectedDeviceCollection();
			initBackupStatusTimer();

			notifier = new Notifier();
			NotifyWebSocketService.Subscribing += notifier.OnSubscribing;
			NotifyWebSocketService.Disconnected += notifier.OnChannelDisconnected;
			notifier.Start();


			ushort backup_port = 0;
			ushort notify_port = 0;
			ushort rest_port = 0;
			try
			{
				backup_port = initWebsocketServer<InfiniteStorageWebSocketService>(out backup_server, 13895);
				notify_port = initWebsocketServer<NotifyWebSocketService>(out notify_server, 13995);
				rest_port = initRestApiServer(out rest_server, 14005);
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger("main").Error("Unable to start web socket or rest api server", e);
				MessageBox.Show(e.Message, Resources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}



			try
			{
				// Stop nginx here to delete possible old nginx instance.
				// Not doing so, bounjour sevice name could become "pc name (2)"...
				NginxUtility.Instance.Stop(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName));

				m_bonjourService = new BonjourService();
				m_bonjourService.Error += new EventHandler<BonjourErrorEventArgs>(m_bonjourService_Error);


				m_bonjourService.Register(backup_port, notify_port, rest_port, Settings.Default.ServerId);

				// bonjour record sometimes disappear for unknown reason.
				// to workaround this, we just periodically re-register with
				// bonjour
				startTimerToReRegisterWithBonjour(backup_port, notify_port, rest_port);

			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger("main").Error("Unable to register bonjour service", e);
				MessageBox.Show(e.Message, "Unable to register bonjour service", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}



			if (Settings.Default.IsFirstUse)
			{
				FirstUseDialog.Instance.Show();
			}
			else
			{
				ThumbnailCreator.Instance.Start();
			}


			ProgramIPC.Instance.OnWinMsg += (s, e) =>
			{
				if (e.Message == ProgramIPC.MsgShowTooltip)
					showProgramIsAtServiceBallonTips();
			};

			var updator = new Waveface.Common.AutoUpdate(false);
			updator.StartLoop();

			NginxUtility.Instance.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName));
			PendingFileMonitor.Instance.Start();
			Application.Run();
		}

		private static void startTimerToReRegisterWithBonjour(ushort backup_port, ushort notify_port, ushort rest_port)
		{
			m_ReRegBonjourTimer = new System.Timers.Timer(60 * 1000);
			m_ReRegBonjourTimer.AutoReset = true;
			m_ReRegBonjourTimer.Elapsed += (s, e) =>
			{
				try
				{
					m_ReRegBonjourTimer.Enabled = false;

					if (m_bonjourService != null)
					{
						m_bonjourService.Dispose();
						System.Threading.Thread.Sleep(500);
					}
					m_bonjourService = new BonjourService();
					m_bonjourService.Error += m_bonjourService_Error;
					m_bonjourService.Register(backup_port, notify_port, rest_port, Settings.Default.ServerId);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger("main").Error("Unable to register bonjour service", err);
					m_bonjourService = null;
				}
				finally
				{
					m_ReRegBonjourTimer.Enabled = true;
				}
			};
			m_ReRegBonjourTimer.Start();
		}

		private static string generateSameServerIdForSameUserOnSamePC()
		{
			string serialNum = getMachineSerialNo();

			var md5 = MD5.Create().ComputeHash(Encoding.Default.GetBytes(serialNum + Environment.UserName + Environment.MachineName));
			return new Guid(md5).ToString();
		}

		private static string getMachineSerialNo()
		{
			string serialNum = null;
			ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
			foreach (ManagementObject getserial in MOS.Get())
			{
				serialNum = getserial["SerialNumber"].ToString();
			}
			return serialNum;
		}

		private static ushort initWebsocketServer<T>(out WebSocketServer<T> server, ushort preferred_port) where T : WebSocketService, new()
		{
			bool addrUsedByOthers = false;
			var port = preferred_port;

			do
			{
				try
				{

					var url = string.Format("ws://0.0.0.0:{0}/", port);
					server = new WebSocketSharp.Server.WebSocketServer<T>(url);
					server.Start();
					return port;
				}
				catch (System.Net.Sockets.SocketException e)
				{
					if (e.SocketErrorCode == System.Net.Sockets.SocketError.AddressAlreadyInUse)
					{
						addrUsedByOthers = true;
						port++;
					}
					else
						throw;
				}
			}
			while (addrUsedByOthers);


			throw new Exception("should not reach here");
		}

		private static ushort initRestApiServer(out HttpServer rest_server, ushort port)
		{
			rest_server = new HttpServer(port);
			rest_server.AddHandler("/image/", new ImageApiHandler());
			rest_server.AddHandler("/label/list_all", new LabelListApiHandler());
			rest_server.AddHandler("/label/tag", new LabelTagApiHandler());
			rest_server.AddHandler("/label/untag", new LabelUntagApiHandler());
			rest_server.AddHandler("/label/clear", new LabelClearApiHandler());
			rest_server.AddHandler("/label/rename", new LabelRenameApiHandler());
			rest_server.AddHandler("/label/add", new LabelAddApiHandler());
			rest_server.AddHandler("/label/delete", new LabelDeleteApiHandler());
			rest_server.AddHandler("/label/get", new LabelGetApiHandler());
			rest_server.AddHandler("/file/get", new FileGetApiHandler());
			rest_server.Start();

			return port;
		}

		private static void invokeAnotherRunningProcess()
		{
			try
			{
				ProgramIPC.SendMessageToSameUserProc(ProgramIPC.MsgShowTooltip);
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger("main").Warn("Unable to invoke another process to show tooltips", e);
			}
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
			m_notifyIconController.refreshNotifyIconContextMenu();
		}

		private static void initNotifyIcon()
		{
			m_notifyIcon = new NotifyIcon();
			m_notifyIcon.Text = Resources.ProductName;
			m_notifyIcon.Icon = Resources.ProductIcon;
			m_notifyIconController = new NotifyIconController(m_notifyIcon);

			m_notifyIcon.ContextMenuStrip = new ContextMenuStrip();

			var openStorageItem = m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_OpenBackupFolder, null, m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked);
			openStorageItem.Font = new Font(openStorageItem.Font, FontStyle.Bold);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Preferences, null, m_notifyIconController.OnPreferencesMenuItemClicked);

			m_notifyIcon.ContextMenuStrip.Items.Add("-");

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Quit, null, m_notifyIconController.OnQuitMenuItemClicked);

			m_notifyIcon.Visible = true;

			showProgramIsAtServiceBallonTips();

			m_notifyIcon.DoubleClick += m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked;
			InfiniteStorageWebSocketService.DeviceAccepted += m_notifyIconController.OnDeviceConnected;
			InfiniteStorageWebSocketService.DeviceDisconnected += m_notifyIconController.OnDeviceDisconnected;
			InfiniteStorageWebSocketService.PairingRequesting += m_notifyIconController.OnDevicePairingRequesting;
			InfiniteStorageWebSocketService.TotalCountUpdated += m_notifyIconController.OnTotalCountUpdated;
		}

		private static void showProgramIsAtServiceBallonTips()
		{
			var ballonText = string.Format(Resources.BallonText_AtService, Resources.ProductName);
			m_notifyIcon.ShowBalloonTip(3000, Resources.ProductName, ballonText, ToolTipIcon.None);
		}

		private static void initConnectedDeviceCollection()
		{
			InfiniteStorageWebSocketService.DeviceAccepted += (s, e) => { ConnectedClientCollection.Instance.Add(e.ctx); };
			InfiniteStorageWebSocketService.DeviceDisconnected += (s, e) => { ConnectedClientCollection.Instance.Remove(e.ctx); };
		}

		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			try
			{
				PendingFileMonitor.Instance.Stop();
				NginxUtility.Instance.Stop(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName));
				notifier.Stop();
				backup_server.Stop();
				ThumbnailCreator.Instance.Stop();
				if (m_notifyIcon != null)
					m_notifyIcon.Dispose();
			}
			catch
			{
			}
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
			log4net.LogManager.GetLogger("main").Warn("Bonjour DNS operation error: " + e.error.ToString());
		}
	}
}
