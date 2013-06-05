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

namespace InfiniteStorage
{
	static class Program
	{
		static StationServer server;

		private static System.Threading.Mutex m_InstanceMutex { get; set; }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
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

				NginxUtility.Instance.PrepareNginxConfig(12888, Settings.Default.SingleFolderLocation);
			}

			if (string.IsNullOrEmpty(Settings.Default.LibraryName))
			{
				Settings.Default.LibraryName = string.Format(Resources.DefLibraryName, Environment.UserName);
				Settings.Default.Save();
			}

			SynchronizationContextHelper.SetMainSyncContext();
			DBInitializer.InitialzeDatabaseSchema();
			SeqNum.InitFromDB();

			NginxUtility.Instance.Start();

			server = new StationServer();
			server.Start();

			if (hasAnyRegisteredDevice())
			{
				BonjourServiceRegistrator.Instance.Register(false);
			}
			else
			{
				WaitForPairingDialog.Instance.Show();
			}
			
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
		}

		static void m_bonjourService_Error(object sender, BonjourErrorEventArgs e)
		{
			MessageBox.Show("Bonjour DNS operation error: " + e.error, "Error");
			log4net.LogManager.GetLogger("main").Warn("Bonjour DNS operation error: " + e.error.ToString());
		}
	}
}
