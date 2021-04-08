using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Linq;

namespace Systems
{
	public static class SystemFunctions
	{
		public static void CreateSymLink(string Source, string Target, SymLinkType Type = SymLinkType.Directory)
		{
			string text = "";
			try
			{
				if (Type == SymLinkType.Directory)
				{
					Directory.CreateDirectory(Source);
					Directory.CreateDirectory(Target);
				}
				else
				{
					if (!File.Exists(Source)) { File.Create(Source); }
					if (!File.Exists(Source)) { File.Create(Target); }
				}
				text = NativeMethods.GetFinalPathName(Source);
			}
			catch (Exception) { }
			finally
			{
				if (text != Target || string.IsNullOrEmpty(text))
				{
					Directory.Delete(Source, true);
					NativeMethods.CreateSymbolicLink(Source, Target, Type);
				}
			}
		}

		public static bool DeleteSymLink(string path)
		{
			bool output = true;
            if (IsSymLink(path))
            {
				Directory.Delete(path);
            }
			return output;
		}

		public static bool IsSymLink(string filePath)
		{
			bool output = false;

			DirectoryInfo DI = new(filePath);
			bool hasReparse = DI.Attributes.HasFlag(FileAttributes.ReparsePoint);
			if (hasReparse)
			{
                var ptr = NativeMethods.FindFirstFile(filePath, out NativeMethods.WIN32_FIND_DATA wfd);
				output = wfd.dwReserved0 == NativeMethods.IO_REPARSE_TAG_SYMLINK;
				while(NativeMethods.FindNextFile(ptr, out wfd))
                {
					output = (wfd.dwReserved0 == NativeMethods.IO_REPARSE_TAG_SYMLINK);
                }

				ptr.Dispose();
			}

			return output;
		}

		public static string GetFinalPathName(string path)
		{
			return NativeMethods.GetFinalPathName(path);
		}

		public static void KillProcessAndChildren(int pid)
		{
			#pragma warning disable CA1416 // Validate platform compatibility
            ManagementObjectSearcher managementObjectSearcher = new("Select * From Win32_Process Where ParentProcessID=" + pid);
			#pragma warning restore CA1416 // Validate platform compatibility
			#pragma warning disable CA1416 // Validate platform compatibility
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			#pragma warning restore CA1416 // Validate platform compatibility
            foreach (ManagementObject item in managementObjectCollection)
			{
				#pragma warning disable CA1416 // Validate platform compatibility
                KillProcessAndChildren(Convert.ToInt32(item["ProcessID"]));
				#pragma warning restore CA1416 // Validate platform compatibility
            }
			try
			{
				Process processById = Process.GetProcessById(pid);
				processById.Kill();
			}
			catch (ArgumentException)
			{
			}
		}

		public static bool IsDirectoryEmpty(string path)
		{
			return !Directory.EnumerateFileSystemEntries(path).Any();
		}

		public static FolderInformation GetFolderInfo(DirectoryInfo source)
		{
			FolderInformation folderInfo = new();
			folderInfo.FolderCount++;
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				folderInfo.FileCount++;
				folderInfo.Size += fileInfo.Length;
			}
			DirectoryInfo[] directories = source.GetDirectories();
			foreach (DirectoryInfo source2 in directories)
			{
				var tmp = GetFolderInfo(source2);
				folderInfo.Size += tmp.Size;
				folderInfo.FolderCount += tmp.FolderCount;
				folderInfo.FileCount += tmp.FileCount;
			}
			folderInfo.Location = source.FullName;
			return folderInfo;
		}
	}
}
