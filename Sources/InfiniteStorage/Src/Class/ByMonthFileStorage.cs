#region

using System;
using System.IO;
using InfiniteStorage.WebsocketProtocol;

#endregion

namespace InfiniteStorage
{
	internal class ByMonthFileStorage : IFileStorage
	{
		private string devFolderPath;
		private string devName;
		private FileMover fileMover = new FileMover();

		public string MoveToStorage(string tempfile, FileContext file)
		{
			if (file.is_thumbnail)
			{
				var target = Path.Combine(MyFileFolder.Thumbs, file.file_id + ".s92.thumb");

				if (File.Exists(target))
					File.Delete(target);

				File.Move(tempfile, target);

				return target;
			}
			else
			{
				var folder = "";

				if (file.datetime.Kind == DateTimeKind.Utc)
					folder = file.datetime.ToLocalTime().ToString("yyyy-MM");
				else
					folder = file.datetime.ToString("yyyy-MM");

				var targetDir = Path.Combine(devFolderPath, folder);

				if (!Directory.Exists(targetDir))
					Directory.CreateDirectory(targetDir);

				var partial_folder = Path.Combine(devName, folder);
				Manipulation.Manipulation.AddFolderRecord(folder, devName, partial_folder);

				var targetFile = Path.Combine(devFolderPath, folder, file.file_name);
				var result_path = fileMover.Move(tempfile, targetFile);

				if (file.datetime.Kind == DateTimeKind.Utc)
					File.SetLastWriteTimeUtc(result_path, file.datetime);
				else
					File.SetLastWriteTime(result_path, file.datetime);

				return result_path;
			}
		}

		public void setDeviceName(string name)
		{
			devName = name;
			devFolderPath = Path.Combine(MyFileFolder.Photo, name);

			if (!Directory.Exists(devFolderPath))
				Directory.CreateDirectory(devFolderPath);
		}
	}
}