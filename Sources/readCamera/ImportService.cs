using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace readCamera
{
	public enum FileType
	{
		Image = 0,
		Video = 1
	}

	public interface ImportService
	{
		IStorage GetStorage(string deviceId, string deviceName);
	}


	public interface IStorage
	{
		bool IsFileExist(string path);

		void AddToStorage(string filename, FileType type, DateTime time, string path);

		string TempFolder { get; }
	}
}
