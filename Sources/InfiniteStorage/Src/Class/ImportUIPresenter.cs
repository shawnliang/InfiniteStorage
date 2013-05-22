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
		private string UI_PROGRAM_NAME = "Notepad.exe";

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
					runUI(device_id);
				}
				else
				{
					if (!devices.Any(x=>x == device_id))
						devices.AddLast(device_id);
				}
			}
		}

		private void runUI(string device_id)
		{
			runningProc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					Arguments = device_id,
					FileName = UI_PROGRAM_NAME,
				},
				EnableRaisingEvents = true,
			};

			runningProc.Exited += new EventHandler(runningProc_Exited);

			runningProc.Start();
		}

		void runningProc_Exited(object sender, EventArgs e)
		{
			lock (cs)
			{
				runningProc = null;

				if (devices.Any())
				{
					var device = devices.First.Value;
					devices.RemoveFirst();

					runUI(device);
				}
			}


		}
	}
}
