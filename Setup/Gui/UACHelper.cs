using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.ComponentModel;

namespace Gui
{
	class UACHelper
	{

		#region Native API definitions

		const UInt32 TOKEN_DUPLICATE = 0x0002;
		const UInt32 TOKEN_QUERY = 0x0008;
		const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
		const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
		const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
		const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
		const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;

		const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
		const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
		const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
		const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;
		const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";

		[StructLayout(LayoutKind.Sequential)]
		struct LUID
		{
			public uint LowPart;
			public int HighPart;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct TOKEN_PRIVILEGES
		{
			public UInt32 PrivilegeCount;
			public LUID Luid;
			public UInt32 Attributes;
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenProcessToken(IntPtr ProcessHandle,
			UInt32 DesiredAccess, out IntPtr TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool LookupPrivilegeValue(string lpSystemName, string lpName,
			out LUID lpLuid);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
		   [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
		   ref TOKEN_PRIVILEGES NewState,
		   UInt32 Zero,
		   IntPtr Null1,
		   IntPtr Null2);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr hObject);

		[DllImport("user32.dll")]
		static extern IntPtr GetShellWindow();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		[Flags]
		enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VMOperation = 0x00000008,
			VMRead = 0x00000010,
			VMWrite = 0x00000020,
			DupHandle = 0x00000040,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			Synchronize = 0x00100000
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		extern static bool DuplicateTokenEx(
			IntPtr hExistingToken,
			uint dwDesiredAccess,
			ref SECURITY_ATTRIBUTES lpTokenAttributes,
			SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
			TOKEN_TYPE TokenType,
			out IntPtr phNewToken);

		[StructLayout(LayoutKind.Sequential)]
		struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public int bInheritHandle;

			public static SECURITY_ATTRIBUTES Empty()
			{
				return new SECURITY_ATTRIBUTES
				{
					nLength = sizeof(int) * 2 + IntPtr.Size,
					lpSecurityDescriptor = IntPtr.Zero,
					bInheritHandle = 0,
				};
			}
		}

		enum SECURITY_IMPERSONATION_LEVEL
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		enum TOKEN_TYPE
		{
			TokenPrimary = 1,
			TokenImpersonation
		}

		enum LogonFlags
		{
			/// <summary>
			/// Log on, then load the user's profile in the HKEY_USERS registry key. The function
			/// returns after the profile has been loaded. Loading the profile can be time-consuming,
			/// so it is best to use this value only if you must access the information in the 
			/// HKEY_CURRENT_USER registry key. 
			/// NOTE: Windows Server 2003: The profile is unloaded after the new process has been
			/// terminated, regardless of whether it has created child processes.
			/// </summary>
			/// <remarks>See LOGON_WITH_PROFILE</remarks>
			WithProfile = 1,
			/// <summary>
			/// Log on, but use the specified credentials on the network only. The new process uses the
			/// same token as the caller, but the system creates a new logon session within LSA, and
			/// the process uses the specified credentials as the default credentials.
			/// This value can be used to create a process that uses a different set of credentials
			/// locally than it does remotely. This is useful in inter-domain scenarios where there is
			/// no trust relationship.
			/// The system does not validate the specified credentials. Therefore, the process can start,
			/// but it may not have access to network resources.
			/// </summary>
			/// <remarks>See LOGON_NETCREDENTIALS_ONLY</remarks>
			NetCredentialsOnly
		}

		[DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool CreateProcessWithTokenW(
			IntPtr hToken,
			LogonFlags dwLogonFlags,
			string lpApplicationName,
			string lpCommandLine,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct STARTUPINFO
		{
			public Int32 cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;

			public static STARTUPINFO Empty()
			{
				STARTUPINFO si = new STARTUPINFO();

				si.lpReserved = null;
				si.lpDesktop = null;
				si.lpTitle = null;
				si.dwX = 0;
				si.dwY = 0;
				si.dwXSize = 0;
				si.dwYSize = 0;
				si.dwXCountChars = 0;
				si.dwYCountChars = 0;
				si.dwFillAttribute = 0;
				si.dwFlags = 0;
				si.wShowWindow = 0;
				si.cbReserved2 = 0;
				si.lpReserved2 = IntPtr.Zero;
				si.hStdInput = IntPtr.Zero;
				si.hStdOutput = IntPtr.Zero;
				si.hStdError = IntPtr.Zero;
				si.cb = Marshal.SizeOf(si);

				return si;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		#endregion

		public static Process CreateProcessAsStandardUser(string file, string args)
		{
			// If windows XP/2000 which has no UAC, no need to adjust security token. 
			// Just create a process and return.
			if (Environment.OSVersion.Version.Major < 6)
				return Process.Start(file, args);


			//The following implementation is roughly based on Aaron Margosis' post:
			//http://blogs.msdn.com/aaron_margosis/archive/2009/06/06/faq-how-do-i-start-a-program-as-the-desktop-user-from-an-elevated-app.aspx
			//and User Access Control Helpers: 
			//http://uachelpers.codeplex.com/

			IntPtr procToken = IntPtr.Zero;
			IntPtr hShellProcess = IntPtr.Zero;
			IntPtr hShellProcessToken = IntPtr.Zero;
			IntPtr hPrimaryToken = IntPtr.Zero;

			try
			{
				procToken = openProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES);
				enableIncreaseQuotaPrivilege(procToken);

				uint dwShellPID = GetShellProcessId();
				//Open the desktop shell process in order to get the process token.
				hShellProcess = openProcess(ProcessAccessFlags.QueryInformation, false, dwShellPID);
				//Get the process token of the desktop shell.
				hShellProcessToken = openProcessToken(hShellProcess, TOKEN_DUPLICATE);

				//Duplicate the shell's process token to get a primary token.
				SECURITY_ATTRIBUTES nullSA = SECURITY_ATTRIBUTES.Empty();
				uint dwTokenRights = TOKEN_QUERY | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID;
				hPrimaryToken = duplicateTokenEx(hShellProcessToken, dwTokenRights,
					ref nullSA, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary);


				//Start the target process with the new token.
				STARTUPINFO si = new STARTUPINFO();
				si.cb = Marshal.SizeOf(si);


				PROCESS_INFORMATION pi = createProcessWithTokenW(hPrimaryToken, 0,
					file, file + " " + args, 0, IntPtr.Zero, null, ref si);

				CloseHandle(pi.hProcess);
				CloseHandle(pi.hThread);

				return Process.GetProcessById(pi.dwProcessId);

			}
			finally
			{
				if (procToken != IntPtr.Zero)
					CloseHandle(procToken);

				if (hShellProcessToken != IntPtr.Zero)
					CloseHandle(hShellProcessToken);

				if (hPrimaryToken != IntPtr.Zero)
					CloseHandle(hPrimaryToken);

				if (hShellProcess != IntPtr.Zero)
					CloseHandle(hShellProcess);

			}
		}

		private static PROCESS_INFORMATION createProcessWithTokenW(
			IntPtr hToken,
			LogonFlags dwLogonFlags,
			string lpApplicationName,
			string lpCommandLine,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo)
		{
			PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

			bool ret = CreateProcessWithTokenW(hToken,
				dwLogonFlags,
				lpApplicationName,
				lpCommandLine,
				dwCreationFlags,
				lpEnvironment,
				lpCurrentDirectory,
				ref lpStartupInfo,
				out pi);

			if (!ret)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return pi;
		}

		private static IntPtr duplicateTokenEx(
			IntPtr hExistingToken,
			uint dwDesiredAccess,
			ref SECURITY_ATTRIBUTES lpTokenAttributes,
			SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
			TOKEN_TYPE TokenType)
		{
			IntPtr phNewToken;

			bool ret = DuplicateTokenEx(hExistingToken, dwDesiredAccess, ref lpTokenAttributes,
				ImpersonationLevel, TokenType, out phNewToken);

			if (!ret)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return phNewToken;
		}

		private static IntPtr openProcessToken(IntPtr ProcessHandle,
			UInt32 DesiredAccess)
		{
			IntPtr procToken;

			if (!OpenProcessToken(ProcessHandle, DesiredAccess, out procToken))
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return procToken;
		}

		private static IntPtr openProcess(ProcessAccessFlags flags, bool inheritance, uint dwShellPID)
		{
			IntPtr hShellProcess = OpenProcess(flags, inheritance, dwShellPID);

			if (hShellProcess == IntPtr.Zero)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return hShellProcess;
		}

		private static uint GetShellProcessId()
		{
			IntPtr hShellWnd = GetShellWindow();
			if (hShellWnd == IntPtr.Zero)
			{
				throw new InvalidOperationException("Unable to locate shell window; you might be using a custom shell");
			}

			uint dwShellPID = 0;
			GetWindowThreadProcessId(hShellWnd, out dwShellPID);
			if (dwShellPID == 0)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return dwShellPID;
		}

		private static void enableIncreaseQuotaPrivilege(IntPtr procToken)
		{
			TOKEN_PRIVILEGES tkp;
			tkp.PrivilegeCount = 1;
			LookupPrivilegeValue(null, SE_INCREASE_QUOTA_NAME, out tkp.Luid);
			tkp.Attributes = SE_PRIVILEGE_ENABLED;
			AdjustTokenPrivileges(procToken, false, ref tkp, 0, IntPtr.Zero, IntPtr.Zero);

			int err = Marshal.GetLastWin32Error();
			if (0 != err)
				throw new Win32Exception(err);

		}
	}
}
