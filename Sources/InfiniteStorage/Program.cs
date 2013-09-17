#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using InfiniteStorage.Win32;
using Microsoft.Win32;
using NBug;
using log4net;
using Settings = InfiniteStorage.Properties.Settings;

#endregion

namespace InfiniteStorage
{
	internal static class Program
	{
		private static StationServer server;

		private static Mutex m_InstanceMutex { get; set; }

		[STAThread]
		private static void Main()
		{
			if (Environment.GetCommandLineArgs().Contains("--close-all-processes"))
			{
				forceCloseAllProcesses();
				return;
			}

			Boolean bCreatedNew;

			var cultureName = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "Culture", "");
			
			if (!string.IsNullOrEmpty(cultureName))
			{
				var cultureInfo = new CultureInfo(cultureName);
				var currentThread = Thread.CurrentThread;

				currentThread.CurrentCulture = cultureInfo;
				currentThread.CurrentUICulture = cultureInfo;
			}

			m_InstanceMutex = new Mutex(false, Application.ProductName + Environment.UserName, out bCreatedNew);

			if (!bCreatedNew)
			{
				invokeAnotherRunningProcess();
				return;
			}

			Application.EnableVisualStyles();

			Application.ApplicationExit += Application_ApplicationExit;

			AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
			Application.ThreadException += Handler.ThreadException;

			Log4netConfigure.InitLog4net();

			LogManager.GetLogger("main").Warn("==== program started ====");

			if (!Settings.Default.IsUpgraded)
			{
				Settings.Default.Upgrade();
				Settings.Default.IsUpgraded = true;
				Settings.Default.Save();
			}

			if (string.IsNullOrEmpty(Settings.Default.ServerId))
			{
				Settings.Default.ServerId = Guid.NewGuid().ToString();
				Settings.Default.SingleFolderLocation = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "ResourceFolder", "");
				Settings.Default.OrganizeMethod = (int) OrganizeMethod.YearMonth;
				Settings.Default.LocationType = (int) LocationType.SingleFolder;
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
				LogManager.GetLogger("main").Warn("Unable to write nginx config", err);
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
				LogManager.GetLogger("main").Error(err.Message, err);

				MessageBox.Show(Resources.DBVersionIncompatible, Resources.IncompatibleVersionDetected, MessageBoxButtons.OK, MessageBoxIcon.Error);
				
				return;
			}

			SeqNum.InitFromDB();

			// checking db and file consistency
			var dialog = new MigratingDataDialog();
			var migratingCompleted = false;
			dialog.Show();

			var bg = new BackgroundWorker();
			bg.DoWork += (s, e) =>
				             {
					             ConsistencyChecker.RemoveMissingFilesFromDB();
					             ConsistencyChecker.RemoveMissingFoldersFromDB();
					             ConsistencyChecker.RemoveMissingDevicesFromDB();

					             removeTempFiles();
				             };
			bg.RunWorkerCompleted += (s, e) =>
				                         {
					                         dialog.CloseByApp();
					                         migratingCompleted = true;
				                         };
			bg.RunWorkerAsync();

			while (!migratingCompleted)
				Application.DoEvents();

			if (HomeSharing.Enabled)
				NginxUtility.Instance.Start();

			server = new StationServer();
			server.Start();

			BonjourServiceRegistrator.Instance.Register(false);

			if (!Environment.GetCommandLineArgs().Contains("--minimized"))
				MainUIWrapper.Instance.StartViewer();

			Application.Run();
		}

		private static void removeTempFiles()
		{
			try
			{
				var tmpFolder = new DirectoryInfo(MyFileFolder.Temp);

				if (!tmpFolder.Exists)
					return;

				foreach (var file in tmpFolder.GetFiles())
					file.Delete();

				foreach (var dir in tmpFolder.GetDirectories())
					dir.Delete(true);
			}
			catch (Exception err)
			{
				LogManager.GetLogger("main").Warn("Delete temp files not success", err);
			}
		}

		/*
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
		*/

		private static void invokeAnotherRunningProcess()
		{
			try
			{
				ProgramIPC.SendMessageToSameUserProc(ProgramIPC.MsgShowTooltip);
			}
			catch (Exception e)
			{
				LogManager.GetLogger("main").Warn("Unable to invoke another process to show tooltips", e);
			}
		}

		private static void Application_ApplicationExit(object sender, EventArgs e)
		{
			try
			{
				server.Stop();
			}
			catch (Exception err)
			{
				LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				NginxUtility.Instance.Stop();
			}
			catch (Exception err)
			{
				LogManager.GetLogger("main").Debug("stop error", err);
			}

			try
			{
				MainUIWrapper.Instance.StopViewer();
			}
			catch (Exception err)
			{
				LogManager.GetLogger("main").Debug("stop error", err);
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
			catch (Exception e)
			{
				LogManager.GetLogger("uninstall").Warn("stop nginx error: ", e);
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