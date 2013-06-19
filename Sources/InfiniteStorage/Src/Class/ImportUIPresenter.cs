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
		private Process viewerProcess;

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
				},
			};
			p.Exited += new EventHandler(viewer_exited);
			p.EnableRaisingEvents = true;
			p.Start();
			viewerProcess = p;
		}

		void viewer_exited(object sender, EventArgs e)
		{
			viewerProcess = null;
		}

		private bool isViewerRunning()
		{
			return viewerProcess != null;
		}

		public void StartViewer()
		{
			if (!isViewerRunning())
				runViewerUI();
			else
				activateExistingViewerUI();
		}

		public bool StopViewer()
		{
			if (viewerProcess == null)
				return true;

			viewerProcess.CloseMainWindow();
			if (viewerProcess.WaitForExit(500))
				return true;

			viewerProcess.Kill();
			return viewerProcess.WaitForExit(500);
		}

		private void activateExistingViewerUI()
		{
			//throw new NotImplementedException();
		}
	}
}
