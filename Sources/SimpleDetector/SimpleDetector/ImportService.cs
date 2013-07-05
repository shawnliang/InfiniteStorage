using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleDetector
{
	public enum FileType
	{
		Image = 0,
		Video = 1
	}

	public interface ImportService
	{
		bool device_exist(string device_id);

        void create_device(string device_id, string device_name);

		bool is_file_exist(string device_id, string file_path);

		void copy_file(System.IO.Stream input, string file_path, SimpleDetector.FileType type, DateTime time, string device_id);
	}
}
