using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage
{
	class ByMonthFileStorage : IFileStorage
	{
		private string devFolderPath;
		private string devName;
		private FileMover fileMover = new FileMover();

		public string MoveToStorage(string tempfile, WebsocketProtocol.FileContext file)
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
			using (var db = new MyDbContext())
			{
				var q = from f in db.Object.Folders
						where f.path.Equals(partial_folder)
						select f;

				if (!q.Any())
				{
					db.Object.Folders.Add(new Folder { path = partial_folder, name = folder, parent_folder = devName });
					db.Object.SaveChanges();
				}
			}

			var targetFile = Path.Combine(devFolderPath, folder, file.file_name);
			return fileMover.Move(tempfile, targetFile);
		}

		public void setDeviceName(string name)
		{
			devName = name;
			devFolderPath = Path.Combine(MyFileFolder.Photo, name);
		}
	}
}
