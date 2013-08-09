using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using InfiniteStorage.Data;

namespace InfiniteStorage
{
	class MainUIWrapper
	{
		private static MainUIWrapper instance = new MainUIWrapper();
		private string VIEWER_UI_PROGRAM = "Waveface.Client";
		private Process viewerProcess;

		private MainUIWrapper()
		{
		}

		public static MainUIWrapper Instance
		{
			get { return instance; }
		}

		private static string getProgDir()
		{
			var progDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return progDir;
		}

		private void runViewerUI(string device_id = null)
		{
			Process p = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = Path.Combine(getProgDir(), VIEWER_UI_PROGRAM + ".exe")
				},
			};

			if (!string.IsNullOrEmpty(device_id))
				p.StartInfo.Arguments = "--select-device " + device_id;

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

		public void StartViewer(string device_id = null)
		{
			if (!isViewerRunning())
				runViewerUI(device_id);
			else
				activateExistingViewerUI(device_id);
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


		private void activateExistingViewerUI(string device_id)
		{
			try
			{
				if (viewerProcess != null)
				{
					NativeMethods.ShowWindow(viewerProcess.MainWindowHandle, NativeMethods.WindowShowStyle.Restore);
					NativeMethods.SetForegroundWindow(viewerProcess.MainWindowHandle);
					instructUiToGoToDevice(device_id);
				}
			}
			catch(Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to activate UI", err);
			}
		}

		private static void instructUiToGoToDevice(string device_id)
		{
			if (!string.IsNullOrEmpty(device_id))
			{
				var wnd = NativeMethods.FindWindow(IPCData.UI_CLASS_NAME, null);
				if (wnd != IntPtr.Zero)
				{
					ProgramIPC.SendStringMsg(wnd, 0, device_id, (int)InfiniteStorage.Data.CopyDataType.JUMP_TO_DEVICE_NODE);
				}
			}
		}
	}
}
