using InfiniteStorage.Properties;
using System;
using System.IO;

namespace InfiniteStorage
{
	class DefaultFolderFileStorage : IFileStorage
	{
		private string storage_path;
		private string devFolder;
		private string partial_path;

		private FileMover mover = new FileMover();

		public string MoveToStorage(string tempfile, WebsocketProtocol.FileContext file)
		{
			if (string.IsNullOrEmpty(storage_path))
				throw new InvalidOperationException("storage path is not set");

			createDefaultFolderIfNotExist();

			return mover.Move(tempfile, Path.Combine(storage_path, file.file_name));
		}

		public void setDeviceName(string devFolder)
		{
			if (string.IsNullOrEmpty(devFolder))
				throw new ArgumentNullException(devFolder);

			this.devFolder = devFolder;
			this.partial_path = Path.Combine(this.devFolder, Resources.UnsortedFolderName);
			this.storage_path = Path.Combine(MyFileFolder.Photo, partial_path);

			createDefaultFolderIfNotExist();
		}

		private void createDefaultFolderIfNotExist()
		{
			if (!Directory.Exists(storage_path))
				Directory.CreateDirectory(storage_path);

			Manipulation.Manipulation.AddFolderRecord(Resources.UnsortedFolderName, this.devFolder, partial_path);
		}
	}
}
