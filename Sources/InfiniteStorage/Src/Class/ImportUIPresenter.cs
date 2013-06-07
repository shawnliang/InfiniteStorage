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
		private string VIEWER_UI_PROGRAM = "Waveface.Client";

		private ImportUIPresenter()
		{
		}

		public static ImportUIPresenter Instance
		{
			get { return instance; }
		}

		private static string getProgDir()
		{
			var progDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return progDir;
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
			else
				activateExistingViewerUI();

		}

		private void activateExistingViewerUI()
		{
			//throw new NotImplementedException();
		}
	}
}
