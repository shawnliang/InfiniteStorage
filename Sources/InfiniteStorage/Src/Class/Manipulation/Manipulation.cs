using InfiniteStorage.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace InfiniteStorage.Manipulation
{
	class Manipulation
	{
		public static event EventHandler<FolderEventArgs> FolderAdded;

		static FileMover fileMover = new FileMover();

		public static MoveResult Move(List<Guid> files, string full_target_path)
		{
			if (!full_target_path.StartsWith(MyFileFolder.Photo))
				throw new ArgumentException("Invalid target path: " + full_target_path);

			var partial_taget_path = PathUtil.MakeRelative(full_target_path, MyFileFolder.Photo);

			var filesToMove = GetFilesById(files);

			if (!Directory.Exists(full_target_path))
			{
				Directory.CreateDirectory(full_target_path);
				AddFolderRecord(Path.GetFileName(partial_taget_path), Path.GetDirectoryName(partial_taget_path), partial_taget_path);
			}


			List<AbstractFileToManipulate> movedFiles;
			List<AbstractFileToManipulate> notMovedFiles;
			moveFiles(full_target_path, filesToMove, out movedFiles, out notMovedFiles);

			updateMovedFileRecords(movedFiles, partial_taget_path);


			return new MoveResult
			{
				moved_files = movedFiles.Select(x => x.file_id).ToList(),
				not_moved_files = notMovedFiles.Select(x => x.file_id).ToList()
			};
		}


		public static List<AbstractFileToManipulate> GetFilesById(List<Guid> file_ids)
		{
			var ret = new List<AbstractFileToManipulate>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select 0 as kind, saved_path, file_name from files where file_id = @file and deleted = 0 " +
									  "union " +
									  "select 1 as kind, saved_path, file_name from pendingfiles where file_id = @file and deleted = 0";
					cmd.Prepare();

					foreach (var file_id in file_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file_id));
						using (var reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								var saved_path = reader["saved_path"].ToString();
								var kind = (long)reader["kind"];
								var file_name = reader["file_name"].ToString();

								if (kind == 0)
									ret.Add(new FileToManipulate { file_id = file_id, saved_path = saved_path, file_name = file_name });
								else
									ret.Add(new PendingFileToManipulate { file_id = file_id, saved_path = saved_path, file_name = file_name });
							}
						}
					}
				}
			}

			return ret;
		}

		public static List<AbstractFileToManipulate> GetFilesFromFolder(List<string> folders)
		{
			using (var db = new MyDbContext())
			{
				var files = from f in db.Object.Files
							where folders.Contains(f.parent_folder)
							select new FileToManipulate
							{
								file_id = f.file_id,
								saved_path = f.saved_path,
								file_name = f.file_name
							};

				return files.ToList().Cast<AbstractFileToManipulate>().ToList();
			}
		}

		public static void AddFolderRecord(string name, string parent_folder, string path)
		{
			using (var db = new MyDbContext())
			{
				var q = from folder in db.Object.Folders
						where folder.path == path
						select folder;

				if (q.FirstOrDefault() != null)
					return;

				db.Object.Folders.Add(new Folder { path = path, parent_folder = parent_folder, name = name });
				db.Object.SaveChanges();
			}

			raiseFolderAddedEvent(new FolderEventArgs { name = name, parent_folder = parent_folder, path = path });
		}

		public static ICollection<Guid> RemoveLabelFiles(IEnumerable<Guid> file_ids)
		{
			var affectedLabels = queryAffectedLabels(file_ids);

			if (affectedLabels.Any())
			{
				deleteLabelFiles(file_ids);
				changeLabelSeqNum(affectedLabels);
			}

			return affectedLabels;
		}

		private static void raiseFolderAddedEvent(FolderEventArgs args)
		{
			var handler = FolderAdded;

			if (handler != null)
				handler(args, args);
		}


		private static ICollection<Guid> queryAffectedLabels(IEnumerable<Guid> file_ids)
		{
			var affectedLabels = new List<Guid>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select label_id from labelFiles where file_id = @file";
					cmd.Prepare();

					foreach (var file_id in file_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file_id));
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

		private static void deleteLabelFiles(IEnumerable<Guid> file_ids)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "delete from LabelFiles where file_id = @file";
					cmd.Prepare();

					foreach (var file_id in file_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file_id));
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}

		private static void changeLabelSeqNum(ICollection<Guid> affectedLabels)
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

		private static void moveFiles(string targetPath, List<AbstractFileToManipulate> filesToMove, out List<AbstractFileToManipulate> movedFiles, out List<AbstractFileToManipulate> notMovedFiles)
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
					log4net.LogManager.GetLogger(typeof(Manipulation)).Warn("Unable to move file: " + file.saved_full_path, err);
					notMovedFiles.Add(file);
				}
			}
		}

		private static void updateMovedFileRecords(List<AbstractFileToManipulate> movedFiles, string partial_folder_path)
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
	}

	class MoveResult
	{
		public List<Guid> moved_files { get; set; }
		public List<Guid> not_moved_files { get; set; }
	}


}
