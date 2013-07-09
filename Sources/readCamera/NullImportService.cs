using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace readCamera
{
	class NullImportService : ImportService
	{
		public bool device_exist(string device_id)
		{
			return true;
		}

		public void create_device(string device_id, string device_name)
		{
		}

		public bool is_file_exist(string device_id, string file_path)
		{
			return false;
		}

		public void copy_file(System.IO.Stream input, string file_path, FileType type, DateTime time, string device_id)
		{
		}


		public IStorage GetStorage(string deviceId, string deviceName)
		{
			return new NullStorageDevice();
		}
	}


	class NullStorageDevice : IStorage
	{

		public bool IsFileExist(string path)
		{
			return true;
		}

		public void AddToStorage(string filename, FileType type, DateTime time, string path)
		{
		}

		public string TempFolder
		{
			get { return ""; }
		}
	}
}
