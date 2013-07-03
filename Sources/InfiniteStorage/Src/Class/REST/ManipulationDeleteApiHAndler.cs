using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;
using System.IO;
using System.Data.SQLite;

namespace InfiniteStorage.REST
{
	class ManipulationDeleteApiHAndler : ManipulationApiHandlerBase
	{
		public override void HandleRequest()
		{
			var file_ids = GetIds();
			var folders = GetPaths();


			var filesToDelete = GetFilesById(file_ids);
			filesToDelete.AddRange(GetFilesFromFolder(folders));

			List<AbstractFileToManipulate> notDeletedFiles;
			var pendingDeleteFiles = moveFilesToRecycleBin(filesToDelete, out notDeletedFiles);

			markAsDeleted(pendingDeleteFiles);

			var affectedLabels = queryAffectedLabels(pendingDeleteFiles);

			deleteLabelFiles(pendingDeleteFiles);

			changeLabelSeqNum(affectedLabels);

			var deletedFolders = deleteFoldersIfEmpty(folders);
			deleteFolderRecords(deletedFolders);

			respondSuccess(new
			{
				api_ret_code = 0,
				api_ret_message = "success",
				status = 200,

				deleted_files = pendingDeleteFiles.Select(x => x.file_id).ToList(),
				not_deleted_files = notDeletedFiles.Select(x => x.file_id).ToList(),
				deleted_folders = deletedFolders.Select(x => Path.Combine(MyFileFolder.Photo, x)).ToList(),
				not_deleted_folders = folders.Except(deletedFolders).Select(x => Path.Combine(MyFileFolder.Photo, x)).ToList(),
				affected_labels = affectedLabels
			});

			deleteFiles(pendingDeleteFiles);
		}

		private void deleteFiles(List<AbstractFileToManipulate> pendingDeleteFiles)
		{
			foreach (var file in pendingDeleteFiles)
			{
				file.DeleteRecycleBinFile();
				file.DeleteThumbnails();
			}
		}

		private void deleteFolderRecords(List<string> deletedFolders)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "delete from folders where path = @folder";
					cmd.Prepare();

					foreach (var folder in deletedFolders)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@folder", folder));
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		private List<string> deleteFoldersIfEmpty(List<string> folders)
		{
			var deleted = new List<string>();

			foreach (var folder in folders)
			{
				try
				{
					var folder_path = Path.Combine(MyFileFolder.Photo, folder);

					var dir = new DirectoryInfo(folder_path);

					if (dir.Exists)
						dir.Delete();

					deleted.Add(folder);
				}
				catch (IOException err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to delete folder: " + folder + " " + err.Message);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to delete folder: " + folder, err);
				}
			}

			return deleted;
		}

		
		private void deleteLabelFiles(List<AbstractFileToManipulate> pendingDeleteFiles)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "delete from LabelFiles where file_id = @file";
					cmd.Prepare();

					foreach (var file in pendingDeleteFiles)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		private void changeLabelSeqNum(ICollection<Guid> affectedLabels)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "update Labels set seq = @seq where label_id = @label";
					cmd.Prepare();

					foreach (var label_id in affectedLabels)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@label", label_id));
						cmd.Parameters.Add(new SQLiteParameter("@seq", SeqNum.GetNextSeq()));
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		private ICollection<Guid> queryAffectedLabels(List<AbstractFileToManipulate> pendingDeleteFiles)
		{
			var affectedLabels = new List<Guid>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select label_id from labelFiles where file_id = @file";
					cmd.Prepare();

					foreach (var file in pendingDeleteFiles)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								affectedLabels.Add(reader.GetGuid(0));
							}
						}
					}
				}
			}

			return affectedLabels.Distinct().ToList();
		}

		private void markAsDeleted(List<AbstractFileToManipulate> pendingDeleteFiles)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "update Files set deleted = 1 where file_id = @file; update PendingFiles set deleted = 1 where file_id = @file";
					cmd.Prepare();

					foreach (var file in pendingDeleteFiles)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));

						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		private List<AbstractFileToManipulate> moveFilesToRecycleBin(List<AbstractFileToManipulate> filesToDelete, out List<AbstractFileToManipulate> notDeletedFiles)
		{
			var recycleBinPath = Path.Combine(MyFileFolder.Photo, ".recycleBin");
			if (!Directory.Exists(recycleBinPath))
			{
				var dir = new DirectoryInfo(recycleBinPath);
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}

			notDeletedFiles = new List<AbstractFileToManipulate>();
			var pendingDeleteItems = new List<AbstractFileToManipulate>();

			foreach (var file in filesToDelete)
			{
				try
				{
					if (File.Exists(file.saved_full_path))
					{
						var temp_path = Path.Combine(recycleBinPath, Guid.NewGuid().ToString());

						file.Move(temp_path);
						file.temp_file_path = temp_path;
						pendingDeleteItems.Add(file);
					}
					else
					{
						pendingDeleteItems.Add(file);
					}					
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to move file: " + file.saved_full_path, err);
					notDeletedFiles.Add(file);
				}
			}
			return pendingDeleteItems;
		}
	}


	


	

}
