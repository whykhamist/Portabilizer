using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Systems
{
	internal class NativeMethods
	{
		private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetFinalPathNameByHandle(IntPtr hFile, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.U4)] uint access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] uint flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymLinkType dwFlags);


		public static string GetFinalPathName(string path)
		{
			IntPtr intPtr = CreateFile(path, 8u, FileShare.Read | FileShare.Write | FileShare.Delete, IntPtr.Zero, FileMode.Open, 33554432u, IntPtr.Zero);
			if (intPtr == INVALID_HANDLE_VALUE)
			{
				throw new Win32Exception();
			}
			try
			{
				StringBuilder stringBuilder = new(1024);
				if (GetFinalPathNameByHandle(intPtr, stringBuilder, 1024u, 0u) == 0)
				{
					throw new Win32Exception();
				}
				return stringBuilder.ToString().Replace("\\\\?\\", "");
			}
			finally
			{
				CloseHandle(intPtr);
			}
		}
	}
}
