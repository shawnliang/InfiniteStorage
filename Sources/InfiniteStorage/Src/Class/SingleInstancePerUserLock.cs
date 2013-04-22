using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InfiniteStorage.Properties;
using System.Security;
using System.Diagnostics;

namespace InfiniteStorage
{
	class SingleInstancePerUserLock
	{
		private static SingleInstancePerUserLock instance;
		private FileStream lockFile;

		public string LockFilePath { get; private set; }

		private SingleInstancePerUserLock()
		{
			LockFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Resources.ProductName,
				"lock.file");
		}

		static SingleInstancePerUserLock()
		{
			instance = new SingleInstancePerUserLock();
		}

		public static SingleInstancePerUserLock Instance
		{
			get { return instance; }
		}

		public bool Lock()
		{
			try
			{
				lockFile = new FileStream(LockFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

				int procId = Process.GetCurrentProcess().Id;

				byte[] data = BitConverter.GetBytes(procId);
				lockFile.Write(data, 0, data.Length);
				lockFile.Flush();

				return true;
			}
			catch
			{
				return false;
			}
		}

		public int ReadProcId()
		{
			using (var f = new FileStream(LockFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				var buffer = new byte[10];
				int nread = f.Read(buffer, 0, buffer.Length);

				while (nread < 4)
				{
					var count = f.Read(buffer, nread, buffer.Length - nread);

					if (count == 0)
						throw new InvalidDataException("Data in lock file is incorrect: " + LockFilePath);

					nread += count;
				}

				return BitConverter.ToInt32(buffer, 0);
			}
		}
	}
}
