using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InfiniteStorage
{
	class ImportUIPresenter
	{
		private static ImportUIPresenter instance = new ImportUIPresenter();

		private LinkedList<string> devices = new LinkedList<string>();
		private object cs = new object();
		private Process pendingUIProcess;
		private string PENDING_UI_PROGRAM_NAME = "PendingUI.exe";
		private string VIEWER_UI_PROGRAM = "Waveface.Client";

		private ImportUIPresenter()
		{

		}

		public static ImportUIPresenter Instance
		{
			get { return instance; }
		}

		public void Show(string device_id)
		{
			lock (cs)
			{
				if (pendingUIProcess == null && devices.Count == 0)
				{
					runPendingUI(device_id);
				}
				else
				{
					if (!devices.Any(x => x == device_id))
						devices.AddLast(device_id);
				}
			}
		}

		private void runPendingUI(string device_id)
		{
			pendingUIProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					Arguments = device_id,
					FileName = Path.Combine(getProgDir(), PENDING_UI_PROGRAM_NAME),
				},
				EnableRaisingEvents = true,
			};

			pendingUIProcess.Exited += new EventHandler(pendingUIProcess_Exited);

			pendingUIProcess.Start();
		}

		private static string getProgDir()
		{
			var progDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return progDir;
		}

		void pendingUIProcess_Exited(object sender, EventArgs e)
		{
			try
			{
				lock (cs)
				{
					pendingUIProcess = null;

					if (devices.Any())
					{
						var device = devices.First.Value;
						devices.RemoveFirst();

						runPendingUI(device);
					}
					else
					{
						if (!isViewerRunning())
							runViewerUI();
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("runningProc_Exited failed", err);
			}
		}

		private void runViewerUI()
		{
			Process p = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(getProgDir(), VIEWER_UI_PROGRAM + ".exe")
				}
			};
			p.Start();
		}

		private bool isViewerRunning()
		{
			var proc = Process.GetProcessesByName(VIEWER_UI_PROGRAM);
			return proc.Any();
		}

		public void StartViewer()
		{
			if (!isViewerRunning())
				runViewerUI();

		}
	}
}
