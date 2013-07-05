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
	}
}
