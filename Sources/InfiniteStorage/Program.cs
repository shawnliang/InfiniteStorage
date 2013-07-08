using InfiniteStorage.Model;
using InfiniteStorage.Notify;
using InfiniteStorage.Properties;
using InfiniteStorage.REST;
using InfiniteStorage.Win32;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Wammer.Station;
using WebSocketSharp.Server;
using Waveface.Common;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using readCamera;

namespace InfiniteStorage
{
	static class Program
	{
		static StationServer server;

		private static System.Threading.Mutex m_InstanceMutex { get; set; }
		private static readCamera.readCameraForm _fm1;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (Environment.GetCommandLineArgs().Contains("--close-all-processes"))
			{
				forceCloseAllProcesses();
				return;
			}

			Boolean bCreatedNew;

			var cultureName = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "Culture", "");
			if (!string.IsNullOrEmpty(cultureName))
			{
				var cultureInfo = new CultureInfo(cultureName);
				var currentThread = Thread.CurrentThread;

				currentThread.CurrentCulture = cultureInfo;
				currentThread.CurrentUICulture = cultureInfo;
			}

			m_InstanceMutex = new System.Threading.Mutex(false, Application.ProductName + Environment.UserName, out bCreatedNew);

			if (!bCreatedNew)
			{
				invokeAnotherRunningProcess();
				return;
			}

			Application.EnableVisualStyles();
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

			AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
			Application.ThreadException += NBug.Handler.ThreadException;

			Log4netConfigure.InitLog4net();

			log4net.LogManager.GetLogger("main").Warn("==== program started ====");

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
			}
			try
			{
				NginxUtility.Instance.PrepareNginxConfig(12888, Settings.Default.SingleFolderLocation);
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Warn("Unable to write nginx config", err);
			}

			if (string.IsNullOrEmpty(Settings.Default.LibraryName))
			{
				Settings.Default.LibraryName = string.Format(Resources.DefLibraryName, Environment.UserName);
				Settings.Default.Save();
			}

			SynchronizationContextHelper.SetMainSyncContext();
			try
			{
				DBInitializer.InitialzeDatabaseSchema();
			}
			catch (DBDowngradeException err)
			{
				log4net.LogManager.GetLogger("main").Error(err.Message, err);

				MessageBox.Show(Resources.DBVersionIncompatible, Resources.IncompatibleVersionDetected, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			SeqNum.InitFromDB();

			if (HomeSharing.Enabled)
				NginxUtility.Instance.Start();

			server = new StationServer();
			server.Start();

			BonjourServiceRegistrator.Instance.Register(false);

			if (!Environment.GetCommandLineArgs().Contains("--minimized"))
				ImportUIPresenter.Instance.StartViewer();

            _fm1 = new readCamera.readCameraForm();            // register cameratype device  plugin/remove event
			_fm1.CameraDetected += new EventHandler<CameraDetectedEventArgs>(_fm1_CameraDetected);
            _fm1.startListen();
            

			Application.Run();
		}

		static void _fm1_CameraDetected(object sender, CameraDetectedEventArgs e)
		{
			SynchronizationContextHelper.SendMainSyncContext(() => {
				var dialog = new AskCameraImportDialog(e.Cameras);
				if (dialog.ShowDialog() != DialogResult.OK)
					return;

				e.SelectedCamera = dialog.SelectedCamera;
			});
		}

		private static bool hasAnyRegisteredDevice()
		{
			using (var db = new MyDbContext())
			{
				return (from d in db.Object.Devices
						select d).Any();
			}
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


		static void Application_ApplicationExit(object sender, EventArgs e)
		{
			try
			{
				server.Stop();
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
				ImportUIPresenter.Instance.StopViewer();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				_fm1.StopListen();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger("main").Debug("stop error", err);
			}
		}

		private static void forceCloseAllProcesses()
		{
			try
			{
				int retry = 10;
				while (procExists("nginx") && retry > 0)
				{
					NginxUtility.Instance.Stop();
					Thread.Sleep(500);

					retry--;
				}
			}
			catch(Exception e)
			{
				log4net.LogManager.GetLogger("uninstall").Warn("stop nginx error: ", e);
			}

			forceCloseProcess("Waveface.Client");
			forceCloseProcess("InfiniteStorage");
		}

		private static bool procExists(string procName)
		{
			var procs = Process.GetProcessesByName(procName);
			return procs != null && procs.Any();
		}

		private static void forceCloseProcess(string procName)
		{
			Process[] uiProcesses;
			int retry = 10;

			while ((uiProcesses = Process.GetProcessesByName(procName)) != null && uiProcesses.Any() && retry > 0)
			{
				foreach (var proc in uiProcesses)
				{
					if (proc.Id == Process.GetCurrentProcess().Id)
						continue;

					try
					{
						proc.Kill();
						proc.WaitForExit(500);
					}
					catch
					{
					}
				}

				retry--;
			}
		}
	}
}
