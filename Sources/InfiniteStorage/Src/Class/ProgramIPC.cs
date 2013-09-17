#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using InfiniteStorage.Win32;

#endregion

namespace InfiniteStorage
{
	internal class ProgramIPC
	{
		public const int MsgShowTooltip = 0x401;

		private const string CLS_NAME_PREFIX = "infiniteStorage.";

		private static ProgramIPC instance;
		private MessageReceiver msgReceiver;

		public event EventHandler<MessageEventArgs> OnWinMsg
		{
			add { msgReceiver.WndProc += value; }
			remove { msgReceiver.WndProc -= value; }
		}

		private ProgramIPC()
		{
			msgReceiver = new MessageReceiver(CLS_NAME_PREFIX + Environment.UserName, null);
		}

		public static ProgramIPC Instance
		{
			get
			{
				if (instance == null)
					instance = new ProgramIPC();

				return instance;
			}
		}

		public static void SendMessageToSameUserProc(int msg)
		{
			var currentProcess = Process.GetCurrentProcess();
			var processes = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);

			if (processes.Any(process => process.Id != currentProcess.Id))
			{
				var handle = NativeMethods.FindWindow(CLS_NAME_PREFIX + Environment.UserName, null);

				if (handle != IntPtr.Zero)
					NativeMethods.SendMessage(handle, MsgShowTooltip, IntPtr.Zero, IntPtr.Zero);
				else
					throw new Exception("Unable to find msg target");
			}
		}

		public static int SendStringMsg(IntPtr hWnd, int wParam, string msg, int code)
		{
			byte[] sarr = Encoding.Default.GetBytes(msg);

			NativeMethods.COPYDATASTRUCT cds;
			cds.dwData = (IntPtr) code;
			cds.lpData = msg;
			cds.cbData = sarr.Length + 1;

			var result = NativeMethods.SendMessage(hWnd, NativeMethods.WM_COPYDATA, wParam, ref cds);

			return result;
		}
	}
}