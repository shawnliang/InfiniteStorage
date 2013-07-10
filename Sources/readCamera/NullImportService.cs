using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace readCamera
{
	class NullImportService : ImportService
	{
		public IStorage GetStorage(string deviceId, string deviceName)
		{
			return new NullStorageDevice();
		}
	}


	class NullStorageDevice : IStorage
	{

		public bool IsFileExist(string path)
		{
			return false;
		}

		public void AddToStorage(string filename, FileType type, DateTime time, string path)
		{
		}

		public string TempFolder
		{
			get { return @"C:\00000000"; }
		}


		public void Connecting()
		{
		}

		public void Connected()
		{
		}

		public void Completed()
		{
		}
	}
}
