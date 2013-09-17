#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfiniteStorage.Model;

#endregion

namespace InfiniteStorage
{
	internal class ConsistencyChecker
	{
		public static void RemoveMissingFilesFromDB()
		{
			var missingFiles = markMissingFilesAsDeleted();

			if (missingFiles.Any())
				Manipulation.Manipulation.RemoveLabelFiles(missingFiles.Select(x => x.file_id));
		}

		public static void RemoveMissingFoldersFromDB()
		{
			using (var db = new MyDbContext())
			{
				var folders = from f in db.Object.Folders
				              select f;

				foreach (var folder in folders)
				{
					var folder_path = Path.Combine(MyFileFolder.Photo, folder.path);

					if (!Directory.Exists(folder_path))
						db.Object.Folders.Remove(folder);
				}

				db.Object.SaveChanges();
			}
		}

		public static void RemoveMissingDevicesFromDB()
		{
			using (var db = new MyDbContext())
			{
				var devices = from dev in db.Object.Devices
				              where !dev.deleted
				              select dev;

				foreach (var dev in devices)
				{
					var dev_path = Path.Combine(MyFileFolder.Photo, dev.folder_name);

					if (!Directory.Exists(dev_path))
						db.Object.Devices.Remove(dev);
				}

				db.Object.SaveChanges();
			}
		}

		private static List<FileAsset> markMissingFilesAsDeleted()
		{
			var missingFiles = new List<FileAsset>();

			using (var db = new MyDbContext())
			{
				var allFiles = from f in db.Object.Files
				               where !f.deleted && f.has_origin
				               select f;

				foreach (var file in allFiles)
				{
					var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);

					if (!File.Exists(file_path))
						missingFiles.Add(file);
				}

				foreach (var file in missingFiles)
				{
					db.Object.Files.Remove(file);
				}

				db.Object.SaveChanges();
			}

			return missingFiles;
		}
	}
}