using System;
using System.Runtime.InteropServices;

public class NativeMethods
{
	#region Const
	public const int WM_USER = 0x400;
	public const int WM_COPYDATA = 0x004A;
	public const uint GENERIC_READ = 0x80000000;
	public const uint OPEN_EXISTING = 3;
	public const uint FILE_SHARE_READ = 0x00000001;
	public const uint FILE_SHARE_WRITE = 0x00000002;
	public const uint FILE_ATTRIBUTE_NORMAL = 128;
	public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
	#endregion


	#region user32.dll
	[return: MarshalAs(UnmanagedType.Bool)]
	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool PostMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

	[DllImport("user32.dll")]
	public static extern bool BringWindowToTop(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("User32.dll", EntryPoint = "SendMessage")]
	public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

	//For use with WM_COPYDATA and COPYDATASTRUCT
	[DllImport("User32.dll", EntryPoint = "SendMessage")]
	public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

	/// <summary>
	/// Registers the class W.
	/// </summary>
	/// <param name="lpWndClass">The lp WND class.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	public static extern UInt16 RegisterClassW(
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
	public static extern IntPtr CreateWindowExW(
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

	//Used for WM_COPYDATA for string messages
	//[StructLayout(LayoutKind.Sequential)]
	public struct COPYDATASTRUCT
	{
		public IntPtr dwData;
		public int cbData;
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpData;
	}

	/// <summary>
	/// Defs the window proc W.
	/// </summary>
	/// <param name="hWnd">The h WND.</param>
	/// <param name="msg">The MSG.</param>
	/// <param name="wParam">The w param.</param>
	/// <param name="lParam">The l param.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr DefWindowProcW(
		IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
	);

	/// <summary>
	/// Destroys the window.
	/// </summary>
	/// <param name="hWnd">The h WND.</param>
	/// <returns></returns>
	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool DestroyWindow(
		IntPtr hWnd
	);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);

	public enum MessageFilterInfo : uint
	{
		None = 0, AlreadyAllowed = 1, AlreadyDisAllowed = 2, AllowedHigher = 3
	};

	public enum ChangeWindowMessageFilterExAction : uint
	{
		Reset = 0, Allow = 1, DisAllow = 2
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct CHANGEFILTERSTRUCT
	{
		public uint size;
		public MessageFilterInfo info;
	}

	//   HDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient,LPVOID NotificationFilter,DWORD Flags);
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

	/// <summary>Enumeration of the different ways of showing a window using 
	/// ShowWindow</summary>
	public enum WindowShowStyle : uint
	{
		/// <summary>Hides the window and activates another window.</summary>
		/// <remarks>See SW_HIDE</remarks>
		Hide = 0,
		/// <summary>Activates and displays a window. If the window is minimized 
		/// or maximized, the system restores it to its original size and 
		/// position. An application should specify this flag when displaying 
		/// the window for the first time.</summary>
		/// <remarks>See SW_SHOWNORMAL</remarks>
		ShowNormal = 1,
		/// <summary>Activates the window and displays it as a minimized window.</summary>
		/// <remarks>See SW_SHOWMINIMIZED</remarks>
		ShowMinimized = 2,
		/// <summary>Activates the window and displays it as a maximized window.</summary>
		/// <remarks>See SW_SHOWMAXIMIZED</remarks>
		ShowMaximized = 3,
		/// <summary>Maximizes the specified window.</summary>
		/// <remarks>See SW_MAXIMIZE</remarks>
		Maximize = 3,
		/// <summary>Displays a window in its most recent size and position. 
		/// This value is similar to "ShowNormal", except the window is not 
		/// actived.</summary>
		/// <remarks>See SW_SHOWNOACTIVATE</remarks>
		ShowNormalNoActivate = 4,
		/// <summary>Activates the window and displays it in its current size 
		/// and position.</summary>
		/// <remarks>See SW_SHOW</remarks>
		Show = 5,
		/// <summary>Minimizes the specified window and activates the next 
		/// top-level window in the Z order.</summary>
		/// <remarks>See SW_MINIMIZE</remarks>
		Minimize = 6,
		/// <summary>Displays the window as a minimized window. This value is 
		/// similar to "ShowMinimized", except the window is not activated.</summary>
		/// <remarks>See SW_SHOWMINNOACTIVE</remarks>
		ShowMinNoActivate = 7,
		/// <summary>Displays the window in its current size and position. This 
		/// value is similar to "Show", except the window is not activated.</summary>
		/// <remarks>See SW_SHOWNA</remarks>
		ShowNoActivate = 8,
		/// <summary>Activates and displays the window. If the window is 
		/// minimized or maximized, the system restores it to its original size 
		/// and position. An application should specify this flag when restoring 
		/// a minimized window.</summary>
		/// <remarks>See SW_RESTORE</remarks>
		Restore = 9,
		/// <summary>Sets the show state based on the SW_ value specified in the 
		/// STARTUPINFO structure passed to the CreateProcess function by the 
		/// program that started the application.</summary>
		/// <remarks>See SW_SHOWDEFAULT</remarks>
		ShowDefault = 10,
		/// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
		/// that owns the window is hung. This flag should only be used when 
		/// minimizing windows from a different thread.</summary>
		/// <remarks>See SW_FORCEMINIMIZE</remarks>
		ForceMinimized = 11
	}
	#endregion


	#region kernel32.dll
	[DllImport("kernel32.dll")]
	public static extern uint GetCurrentThreadId();

	//[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	//public static extern IntPtr FindFirstFile(string pFileName, ref  WIN32_FIND_DATA pFindFileData);

	//[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	//public static extern bool FindNextFile(IntPtr hndFindFile, ref  WIN32_FIND_DATA lpFindFileData);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool FindClose(IntPtr hndFindFile);


	// should be "static extern unsafe"
	[DllImport("kernel32", SetLastError = true)]
	public static extern IntPtr CreateFile(
		  string FileName,                    // file name
		  uint DesiredAccess,                 // access mode
		  uint ShareMode,                     // share mode
		  IntPtr SecurityAttributes,            // Security Attributes
		  uint CreationDisposition,           // how to create
		  uint FlagsAndAttributes,            // file attributes
		  IntPtr hTemplateFile                   // handle to template file
		  );


	[DllImport("kernel32", SetLastError = true)]
	public static extern bool CloseHandle(
		  IntPtr hObject   // handle to object
		  );
	#endregion


	#region wininet
	[DllImport("wininet")]
	public static extern bool InternetGetConnectedState(
		ref uint lpdwFlags,
		uint dwReserved
		);
	#endregion
}