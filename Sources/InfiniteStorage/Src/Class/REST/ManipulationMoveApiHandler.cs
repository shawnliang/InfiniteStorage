using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using System.IO;
using InfiniteStorage.Model;
using System.Data.SQLite;

namespace InfiniteStorage.REST
{
	class ManipulationMoveApiHandler : ManipulationApiHandlerBase
	{
		FileMover fileMover = new FileMover();

		public override void HandleRequest()
		{
			CheckParameter("targetPath");

			var file_ids = GetIds();
			var folders = GetPaths();
			var full_target_path = Parameters["targetPath"];

			if (!full_target_path.StartsWith(MyFileFolder.Photo))
				throw new ArgumentException("invalid targetPath");


			var partial_taget_path = PathUtil.MakeRelative(full_target_path, MyFileFolder.Photo);

			var filesToMove = GetFilesById(file_ids);
			filesToMove.AddRange(GetFilesFromFolder(folders));

			if (!Directory.Exists(full_target_path))
			{
				Directory.CreateDirectory(full_target_path);
				addFolderRecord(partial_taget_path);
			}


			List<AbstractFileToManipulate> movedFiles;
			List<AbstractFileToManipulate> notMovedFiles;
			moveFiles(full_target_path, filesToMove, out movedFiles, out notMovedFiles);

			updateFileRecords(movedFiles, partial_taget_path);


			respondSuccess(new {
				api_ret_code = 0,
				api_ret_message = "success",
				status = 200,

				moved_files = movedFiles.Select(x=>x.file_id),
				not_moved_files = notMovedFiles.Select(x=>x.file_id)
			});

		}

		private void updateFileRecords(List<AbstractFileToManipulate> movedFiles, string partial_folder_path)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var transaction = conn.BeginTransaction())
				using (var cmdUpdate = conn.CreateCommand())
				using (var cmdInsert = conn.CreateCommand())
				{
					cmdUpdate.CommandText = "update Files set saved_path = @saved, parent_folder = @parent where file_id = @file";
					cmdUpdate.Prepare();

					cmdInsert.CommandText =
						"insert into Files (file_id, file_name, file_path, file_size, saved_path, parent_folder, device_id, type, event_time, seq, deleted, thumb_ready, width, height, orientation) " +
						"select file_id, file_name, file_path, file_size, @saved, @parent, device_id, type, event_time, @seq, deleted, thumb_ready, width, height, orientation from [PendingFiles] " +
						"where file_id = @fid; " +
						
						"delete from pendingFiles where file_id = @fid";
					cmdInsert.Prepare();

					foreach (var file in movedFiles)
					{
						if (file.IsPendingFile)
						{
							cmdInsert.Parameters.Clear();
							cmdInsert.Parameters.Add(new SQLiteParameter("@saved", PathUtil.MakeRelative(file.temp_file_path, MyFileFolder.Photo)));
							cmdInsert.Parameters.Add(new SQLiteParameter("@parent", partial_folder_path));
							cmdInsert.Parameters.Add(new SQLiteParameter("@fid", file.file_id));
							cmdInsert.Parameters.Add(new SQLiteParameter("@seq", SeqNum.GetNextSeq()));
							cmdInsert.ExecuteNonQuery();
						}
						else
						{
							cmdUpdate.Parameters.Clear();
							cmdUpdate.Parameters.Add(new SQLiteParameter("@saved", PathUtil.MakeRelative(file.temp_file_path, MyFileFolder.Photo)));
							cmdUpdate.Parameters.Add(new SQLiteParameter("@parent", partial_folder_path));
							cmdUpdate.Parameters.Add(new SQLiteParameter("@file", file.file_id));
							cmdUpdate.ExecuteNonQuery();
						}
					}

					transaction.Commit();
				}
			}
		}

		private void moveFiles(string targetPath, List<AbstractFileToManipulate> filesToMove, out List<AbstractFileToManipulate> movedFiles, out List<AbstractFileToManipulate> notMovedFiles)
		{
			movedFiles = new List<AbstractFileToManipulate>();
			notMovedFiles = new List<AbstractFileToManipulate>();

			foreach (var file in filesToMove)
			{
				try
				{
					var newPath = Path.Combine(targetPath, file.file_name);
					
					file.temp_file_path = fileMover.Move(file.saved_full_path, newPath);

					movedFiles.Add(file);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to move file: " + file.saved_full_path, err);
					notMovedFiles.Add(file);
				}
			}
		}

		private void addFolderRecord(string targetPath)
		{
			using (var db = new MyDbContext())
			{
				var q = from folder in db.Object.Folders
						where folder.path == targetPath
						select folder;

				if (q.FirstOrDefault() != null)
					return;

				db.Object.Folders.Add(new Folder { path = targetPath, parent_folder = Path.GetDirectoryName(targetPath), name = Path.GetFileName(targetPath) });
				db.Object.SaveChanges();
			}
		}

		private string createTempFolder()
		{
			string tempPath;

			do
			{
				tempPath = Path.Combine(MyFileFolder.Photo, "." + Guid.NewGuid().ToString());

			} while (Directory.Exists(tempPath));


			var dir = new DirectoryInfo(tempPath);
			dir.Create();
			dir.Attributes |= FileAttributes.Hidden;

			return tempPath;
		}
	}
}
