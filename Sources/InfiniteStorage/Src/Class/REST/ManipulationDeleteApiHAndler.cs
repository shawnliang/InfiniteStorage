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
	class ManipulationDeleteApiHAndler : HttpHandler
	{
		public override void HandleRequest()
		{
			var file_ids = parseToList(Parameters["ids"]).Select(x => new Guid(x)).ToList();
			var folders = parseToList(Parameters["paths"]).Select(x => PathUtil.MakeRelative(x, MyFileFolder.Photo)).ToList();


			var filesToDelete = getFiles(file_ids);
			filesToDelete.AddRange(getFilesFromFolders(folders));

			var pendingDeleteFiles = moveFilesToRecycleBin(filesToDelete);

			markAsDeleted(pendingDeleteFiles);

			var affectedLabels = queryAffectedLabels(pendingDeleteFiles);

			deleteLabelFiles(pendingDeleteFiles);

			changeLabelSeqNum(affectedLabels);

			var deletedFolders = deleteFoldersIfEmpty(folders);
			deleteFolderRecords(deletedFolders);

			respondSuccess();

			deleteFiles(pendingDeleteFiles);
		}

		private void deleteFiles(List<FileToDelete> pendingDeleteFiles)
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

		private List<FileToDelete> getFilesFromFolders(List<string> folders)
		{
			using (var db = new MyDbContext())
			{
				var files = from f in db.Object.Files
							where folders.Contains(f.parent_folder)
							select new FileToDelete
							{
								file_id = f.file_id,
								saved_path = f.saved_path
							};
				return files.ToList();
			}
		}

		private void deleteLabelFiles(List<FileToDelete> pendingDeleteFiles)
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

		private ICollection<Guid> queryAffectedLabels(List<FileToDelete> pendingDeleteFiles)
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

		private void markAsDeleted(List<FileToDelete> pendingDeleteFiles)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "update Files set deleted = 1 where file_id = @file";
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

		private List<FileToDelete> moveFilesToRecycleBin(List<FileToDelete> filesToDelete)
		{
			var pendingDeleteItems = new List<FileToDelete>();

			foreach (var file in filesToDelete)
			{
				var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);

				try
				{
					if (!File.Exists(file_path))
						continue;

					file.recycle_bin_path = moveToRecycleBin(file_path);
					pendingDeleteItems.Add(file);
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to move file: " + file_path, err);
				}
			}
			return pendingDeleteItems;
		}

		private string moveToRecycleBin(string file_path)
		{
			var recycleBinPath = Path.Combine(MyFileFolder.Photo, ".recycleBin");
			if (!Directory.Exists(recycleBinPath))
			{
				var dir = new DirectoryInfo(recycleBinPath);
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}

			var temp_path = Path.Combine(recycleBinPath, Guid.NewGuid().ToString());
			File.Move(file_path, temp_path);

			return temp_path;
		}

		private string[] parseToList(string para)
		{
			if (string.IsNullOrEmpty(para))
				return new string[] { };

			return para.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}


		private List<FileToDelete> getFiles(List<Guid> file_ids)
		{
			var ret = new List<FileToDelete>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select file_id, saved_path from files where file_id = @file";
					cmd.Prepare();

					foreach (var file_id in file_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@file", file_id));
						using (var reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								ret.Add(new FileToDelete { file_id = (Guid)reader["file_id"], saved_path = reader["saved_path"].ToString() });
							}
						}
					}
				}
			}

			return ret;
		}
	}


	internal class FileToDelete
	{
		public Guid file_id { get; set; }
		public string saved_path { get; set; }
		public string recycle_bin_path { get; set; }

		public string small_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string medium_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string large_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public string tiny_thumb_path
		{
			get { return Path.Combine(MyFileFolder.Thumbs, file_id + ".small.thumb"); }
		}

		public void DeleteThumbnails()
		{
			var small = new FileInfo(small_thumb_path);
			delete(small);

			var medium = new FileInfo(medium_thumb_path);
			delete(medium);

			var large = new FileInfo(large_thumb_path);
			delete(large);

			var tiny = new FileInfo(tiny_thumb_path);
			delete(tiny);
		}

		public void DeleteRecycleBinFile()
		{
			delete(new FileInfo(recycle_bin_path));
		}

		private void delete(FileInfo file)
		{
			try
			{
				if (file.Exists)
					file.Delete();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to delete file: " + file.FullName, err);
			}
		}
	}


}
