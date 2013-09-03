using System;

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

		void Connecting();

		void Connected();

		void Completed();
	}
}
