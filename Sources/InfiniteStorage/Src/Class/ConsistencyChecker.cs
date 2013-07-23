using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage
{
	class ConsistencyChecker
	{
		public static void RemoveMissingFilesFromDB()
		{
			var missingFiles = markMissingFilesAsDeleted();

			if (missingFiles.Any())
				Manipulation.Manipulation.RemoveLabelFiles(missingFiles.Select(x=>x.file_id));
		}

		private static List<FileAsset> markMissingFilesAsDeleted()
		{
			var missingFiles = new List<FileAsset>();
			using (var db = new MyDbContext())
			{
				var allFiles = from f in db.Object.Files
							   where !f.deleted
							   select f;


				foreach (var file in allFiles)
				{
					var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);
					if (!File.Exists(file_path))
						missingFiles.Add(file);
				}

				foreach (var file in missingFiles)
				{
					file.deleted = true;
				}

				db.Object.SaveChanges();
			}
			return missingFiles;
		}
	}
}
