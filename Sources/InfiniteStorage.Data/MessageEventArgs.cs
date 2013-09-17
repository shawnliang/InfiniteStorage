#region

using System;

#endregion

namespace InfiniteStorage.Win32
{
	public class MessageEventArgs : EventArgs
	{
		public uint Message { get; set; }
		public IntPtr wParam { get; set; }
		public IntPtr lParam { get; set; }

		public MessageEventArgs(uint message, IntPtr wparam, IntPtr lparam)
		{
			Message = message;
			wParam = wparam;
			lParam = lparam;
		}
	}
}