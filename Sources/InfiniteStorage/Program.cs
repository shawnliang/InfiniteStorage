using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using InfiniteStorage.Properties;
using InfiniteStorage.REST;
using InfiniteStorage.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Wammer.Station;
using WebSocketSharp.Server;
using Microsoft.Win32;

namespace InfiniteStorage
{
	static class Program
	{
		//static BonjourService m_bonjourService;
		static NotifyIcon m_notifyIcon;
		static NotifyIconController m_notifyIconController;
		static Timer m_NotifyTimer;
		static System.Timers.Timer m_BackupStatusTimer;
		static System.Timers.Timer m_ReRegBonjourTimer;
		static WebSocketServer<InfiniteStorageWebSocketService> backup_server;
		static WebSocketServer<NotifyWebSocketService> notify_server;
		static HttpServer rest_server;
		static Notifier notifier;
		static AutoLabelController autoLabel = new AutoLabelController();

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
#if DEBUG
			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
			Application.ThreadException += NBug.Handler.ThreadException;
#endif

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
				Settings.Default.SingleFolderLocation = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
				Settings.Default.OrganizeMethod = (int)OrganizeMethod.YearMonth;
				Settings.Default.LocationType = (int)LocationType.SingleFolder;
				Settings.Default.Save();

				if (!Directory.Exists(Settings.Default.SingleFolderLocation))
					Directory.CreateDirectory(Settings.Default.SingleFolderLocation);

				NginxUtility.Instance.PrepareNginxConfig(12888, Settings.Default.SingleFolderLocation);
			}

			NginxUtility.Instance.Start();
			ThumbnailCreator.Instance.Start();

			SynchronizationContextHelper.SetMainSyncContext();
			DBInitializer.InitialzeDatabaseSchema();
			SeqNum.InitFromDB();

			initNotifyIcon();
			InfiniteStorageWebSocketService.FileReceived += ProgressTooltip.Instance.OnFileEnding;
			InfiniteStorageWebSocketService.FileReceived += autoLabel.FileReceived;

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

			BonjourServiceRegistrator.Instance.SetPorts(backup_port, notify_port, rest_port);


			if (hasAnyRegisteredDevice())
			{
				BonjourServiceRegistrator.Instance.Register(false);
			}
			else
			{
				WaitForPairingDialog.Instance.Show();
				BonjourServiceRegistrator.Instance.Register(true);
			}

			// bonjour record sometimes disappear for unknown reason.
			// to workaround this, we just re-register with bonjour
			// every minute.
			startTimerToReRegisterWithBonjour();

			ProgramIPC.Instance.OnWinMsg += (s, e) =>
			{
				if (e.Message == ProgramIPC.MsgShowTooltip)
					showProgramIsAtServiceBallonTips();
			};

			var updator = new Waveface.Common.AutoUpdate(false);
			updator.StartLoop();


			PendingFileMonitor.Instance.Start();
			Application.Run();
		}

		private static bool hasAnyRegisteredDevice()
		{
			using (var db = new MyDbContext())
			{
				return (from d in db.Object.Devices
						select d).Any();
			}
		}

		private static void startTimerToReRegisterWithBonjour()
		{
			m_ReRegBonjourTimer = new System.Timers.Timer(60 * 1000);
			m_ReRegBonjourTimer.AutoReset = true;
			m_ReRegBonjourTimer.Elapsed += (s, e) =>
			{
				try
				{
					m_ReRegBonjourTimer.Enabled = false;
					BonjourServiceRegistrator.Instance.Register();
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger("main").Error("Unable to register bonjour service", err);
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
			rest_server.AddHandler("/pending/get", new PendingGetApiHandler());
			rest_server.AddHandler("/pending/sort", new PendingSortApiHandler());
			rest_server.AddHandler("/label_cover", new LabelCoverApiHandler());
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

			var openUIItem = m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Open, null, m_notifyIconController.OnOpenUIMenuItemCliecked);
			openUIItem.Font = new Font(openUIItem.Font, FontStyle.Bold);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_OpenBackupFolder, null, m_notifyIconController.OnOpenPhotoBackupFolderMenuItemClicked);

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Preferences, null, m_notifyIconController.OnPreferencesMenuItemClicked);
			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_AddNewSources, null, m_notifyIconController.OnAddingNewSources);
			m_notifyIcon.ContextMenuStrip.Items.Add("-");

			m_notifyIcon.ContextMenuStrip.Items.Add(Resources.TrayMenuItem_Quit, null, m_notifyIconController.OnQuitMenuItemClicked);

			m_notifyIcon.Visible = true;

			showProgramIsAtServiceBallonTips();

			m_notifyIcon.DoubleClick += m_notifyIconController.OnOpenUIMenuItemCliecked;
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
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				NginxUtility.Instance.Stop();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				notifier.Stop();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				backup_server.Stop();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				ThumbnailCreator.Instance.Stop();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				if (m_notifyIcon != null)
					m_notifyIcon.Dispose();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
			log4net.LogManager.GetLogger("main").Warn("Bonjour DNS operation error: " + e.error.ToString());
		}
	}
}
