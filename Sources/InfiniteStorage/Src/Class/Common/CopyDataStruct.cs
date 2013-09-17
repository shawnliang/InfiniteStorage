#region

using System;
using System.Runtime.InteropServices;

#endregion

[StructLayout(LayoutKind.Sequential)]
public struct CopyDataStruct
{
	public IntPtr dwData;
	public int cbData;
	public IntPtr lpData;
}