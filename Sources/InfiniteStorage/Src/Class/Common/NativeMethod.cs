using System;
using System.Runtime.InteropServices;

internal class NativeMethods
{
	#region Const
	internal const int WM_USER = 0x400;
	internal const int WM_COPYDATA = 0x004A;
	internal const uint GENERIC_READ = 0x80000000;
	internal const uint OPEN_EXISTING = 3;
	internal const uint FILE_SHARE_READ = 0x00000001;
	internal const uint FILE_SHARE_WRITE = 0x00000002;
	internal const uint FILE_ATTRIBUTE_NORMAL = 128;
	internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
	#endregion


	#region user32.dll
	[return: MarshalAs(UnmanagedType.Bool)]
	[DllImport("user32.dll", SetLastError = true)]
	internal static extern bool PostMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	internal static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

	[DllImport("user32.dll")]
	internal static extern bool BringWindowToTop(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

	[DllImport("user32.dll")]
	internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("User32.dll", EntryPoint = "SendMessage")]
	internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref CopyDataStruct lParam);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

	/// <summary>
	/// Registers the class W.
	/// </summary>
	/// <param name="lpWndClass">The lp WND class.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	internal static extern UInt16 RegisterClassW(
		[In] ref WNDCLASS lpWndClass
	);

	/// <summary>
	/// Creates the window ex W.
	/// </summary>
	/// <param name="dwExStyle">The dw ex style.</param>
	/// <param name="lpClassName">Name of the lp class.</param>
	/// <param name="lpWindowName">Name of the lp window.</param>
	/// <param name="dwStyle">The dw style.</param>
	/// <param name="x">The x.</param>
	/// <param name="y">The y.</param>
	/// <param name="nWidth">Width of the n.</param>
	/// <param name="nHeight">Height of the n.</param>
	/// <param name="hWndParent">The h WND parent.</param>
	/// <param name="hMenu">The h menu.</param>
	/// <param name="hInstance">The h instance.</param>
	/// <param name="lpParam">The lp param.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr CreateWindowExW(
	   UInt32 dwExStyle,
	   [MarshalAs(UnmanagedType.LPWStr)]
	   string lpClassName,
	   [MarshalAs(UnmanagedType.LPWStr)]
	   string lpWindowName,
	   UInt32 dwStyle,
	   Int32 x,
	   Int32 y,
	   Int32 nWidth,
	   Int32 nHeight,
	   IntPtr hWndParent,
	   IntPtr hMenu,
	   IntPtr hInstance,
	   IntPtr lpParam
	);

	/// <summary>
	/// Defs the window proc W.
	/// </summary>
	/// <param name="hWnd">The h WND.</param>
	/// <param name="msg">The MSG.</param>
	/// <param name="wParam">The w param.</param>
	/// <param name="lParam">The l param.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	internal static extern IntPtr DefWindowProcW(
		IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
	);

	/// <summary>
	/// Destroys the window.
	/// </summary>
	/// <param name="hWnd">The h WND.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	internal static extern bool DestroyWindow(
		IntPtr hWnd
	);

	//   HDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient,LPVOID NotificationFilter,DWORD Flags);
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	internal static extern uint UnregisterDeviceNotification(IntPtr hHandle);
	#endregion


	#region kernel32.dll
	[DllImport("kernel32.dll")]
	internal static extern uint GetCurrentThreadId();

	//[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	//internal static extern IntPtr FindFirstFile(string pFileName, ref  WIN32_FIND_DATA pFindFileData);

	//[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	//internal static extern bool FindNextFile(IntPtr hndFindFile, ref  WIN32_FIND_DATA lpFindFileData);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool FindClose(IntPtr hndFindFile);


	// should be "static extern unsafe"
	[DllImport("kernel32", SetLastError = true)]
	internal static extern IntPtr CreateFile(
		  string FileName,                    // file name
		  uint DesiredAccess,                 // access mode
		  uint ShareMode,                     // share mode
		  IntPtr SecurityAttributes,            // Security Attributes
		  uint CreationDisposition,           // how to create
		  uint FlagsAndAttributes,            // file attributes
		  IntPtr hTemplateFile                   // handle to template file
		  );


	[DllImport("kernel32", SetLastError = true)]
	internal static extern bool CloseHandle(
		  IntPtr hObject   // handle to object
		  );
	#endregion


	#region wininet
	[DllImport("wininet")]
	internal static extern bool InternetGetConnectedState(
		ref uint lpdwFlags,
		uint dwReserved
		);
	#endregion
}