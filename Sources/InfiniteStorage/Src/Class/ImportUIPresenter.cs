using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace InfiniteStorage
{
	class ImportUIPresenter
	{
		private static ImportUIPresenter instance = new ImportUIPresenter();

		private LinkedList<string> devices = new LinkedList<string>();
		private object cs = new object();
		private Process runningProc;
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
				if (runningProc == null && devices.Count == 0)
				{
					runPendingUI(device_id);
				}
				else
				{
					if (!devices.Any(x=>x == device_id))
						devices.AddLast(device_id);
				}
			}
		}

		private void runPendingUI(string device_id)
		{
			runningProc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					Arguments = device_id,
					FileName = PENDING_UI_PROGRAM_NAME,
				},
				EnableRaisingEvents = true,
			};

			runningProc.Exited += new EventHandler(runningProc_Exited);

			runningProc.Start();
		}

		void runningProc_Exited(object sender, EventArgs e)
		{
			try
			{
				lock (cs)
				{
					runningProc = null;

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

					FileName = VIEWER_UI_PROGRAM + ".exe"
				}
			};
			p.Start();
		}

		private bool isViewerRunning()
		{
			var proc = Process.GetProcessesByName(VIEWER_UI_PROGRAM);
			return proc.Any();
		}
	}
}
