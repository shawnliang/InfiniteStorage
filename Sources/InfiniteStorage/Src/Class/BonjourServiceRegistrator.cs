#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using InfiniteStorage.Properties;

#endregion

namespace InfiniteStorage
{
	internal class BonjourServiceRegistrator
	{
		private static BonjourServiceRegistrator instance = new BonjourServiceRegistrator();

		private ushort backup_port;
		private ushort notify_port;
		private ushort rest_port;
		private bool is_accepting;

		private Process agentProcess;

		private object cs = new object();

		private BonjourServiceRegistrator()
		{
			var rand = new Random();
			var code = rand.Next(10000);
			Passcode = code.ToString("0000");
		}

		public static BonjourServiceRegistrator Instance
		{
			get { return instance; }
		}

		public void SetPorts(ushort backup, ushort notify, ushort rest)
		{
			backup_port = backup;
			notify_port = notify;
			rest_port = rest;
		}

		public string Passcode { get; private set; }

		public void Register(bool? isAccepting = null)
		{
			lock (cs)
			{
				if (isAccepting.HasValue)
					is_accepting = isAccepting.Value;

				if (agentProcess != null)
				{
					agentProcess.StandardInput.Write("quit");
					agentProcess.WaitForExit(1000);
					agentProcess = null;

					Thread.Sleep(1000);
				}

				var procs = Process.GetProcessesByName("BonjourAgent");

				if (procs != null && procs.Any())
					killProc(procs);

				var otherArg = string.Empty;

				if (is_accepting)
					otherArg += " --is-accepting";

				if (HomeSharing.Enabled)
					otherArg += " --home-sharing";

				agentProcess = new Process();
				agentProcess.StartInfo = new ProcessStartInfo
					                         {
						                         FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BonjourAgent.exe"),
						                         Arguments = string.Format("--server-name \"{0}\" --server-id {1} --backup-port {2} --notify-port {3} --rest-port {4} --passcode {5}",
						                                                   Settings.Default.LibraryName, Settings.Default.ServerId, backup_port, notify_port, rest_port, Passcode) + otherArg,
						                         CreateNoWindow = true,
						                         WindowStyle = ProcessWindowStyle.Hidden,
						                         RedirectStandardInput = true,
						                         UseShellExecute = false
					                         };

				agentProcess.Start();
			}
		}

		private void killProc(Process[] procs)
		{
			foreach (var proc in procs)
			{
				try
				{
					proc.Kill();
					proc.WaitForExit(1000);
				}
				catch
				{
				}
			}
		}

		public bool IsAccepting
		{
			get { return is_accepting; }
		}
	}
}