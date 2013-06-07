using InfiniteStorage.WebsocketProtocol;
using System;
using System.IO;

namespace InfiniteStorage
{
	class PendingFileStorage : IFileStorage
	{
		// using lazy filename because file storage might not be set when this instance create (first use scenario).
		Lazy<string> storage_path;
		IFileMove fileMover = new FileMover();

		public PendingFileStorage()
		{
			storage_path = new Lazy<string>(getPendingFileStorage);
		}

		private string getPendingFileStorage()
		{
			var path = Path.Combine(MyFileFolder.Photo, ".pending");

			if (!Directory.Exists(path))
			{
				var dir = new DirectoryInfo(path);
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}

			return path;
		}

		public SavedPath MoveToStorage(string tempfile, FileContext file)
		{
			var destName = Path.Combine(storage_path.Value, Path.GetFileName(tempfile) + Path.GetExtension(file.file_name));

			string saved_file_path = fileMover.Move(tempfile, destName);

			return new SavedPath { device_folder = storage_path.Value, relative_file_path = Path.GetFileName(saved_file_path) };
		}

		public void setDeviceName(string name)
		{
		}
	}
}
