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

		private List<AbstractFileToManipulate> getFilesFromFolders(List<string> folders)
		{
			using (var db = new MyDbContext())
			{
				var files = from f in db.Object.Files
							where folders.Contains(f.parent_folder)
							select new FileToManipulate
							{
								file_id = f.file_id,
								saved_path = f.saved_path
							};
				return files.ToList().Cast<AbstractFileToManipulate>().ToList();
			}
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

		private List<AbstractFileToManipulate> moveFilesToRecycleBin(List<AbstractFileToManipulate> filesToDelete)
		{
			var recycleBinPath = Path.Combine(MyFileFolder.Photo, ".recycleBin");
			if (!Directory.Exists(recycleBinPath))
			{
				var dir = new DirectoryInfo(recycleBinPath);
				dir.Create();
				dir.Attributes |= FileAttributes.Hidden;
			}

			var pendingDeleteItems = new List<AbstractFileToManipulate>();

			foreach (var file in filesToDelete)
			{
				try
				{
					if (File.Exists(file.saved_full_path))
					{
						var temp_path = Path.Combine(recycleBinPath, Guid.NewGuid().ToString());

						file.Move(temp_path);
						file.recycle_bin_path = temp_path;
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
				}
			}
			return pendingDeleteItems;
		}

		private string[] parseToList(string para)
		{
			if (string.IsNullOrEmpty(para))
				return new string[] { };

			return para.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}


		private List<AbstractFileToManipulate> getFiles(List<Guid> file_ids)
		{
			var ret = new List<AbstractFileToManipulate>();

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "select 0 as kind, saved_path from files where file_id = @file and deleted = 0 " +
									  "union " +
									  "select 1 as kind, saved_path from pendingfiles where file_id = @file and deleted = 0";
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

								if (kind == 0)
									ret.Add(new FileToManipulate { file_id = file_id, saved_path = saved_path });
								else
									ret.Add(new PendingFileToManipulate { file_id = file_id, saved_path = saved_path });
							}
						}
					}
				}
			}

			return ret;
		}
	}


	internal abstract class AbstractFileToManipulate
	{
		public Guid file_id { get; set; }
		public string saved_path { get; set; }
		public string recycle_bin_path { get; set; }

		public abstract string saved_full_path { get; }

		protected AbstractFileToManipulate()
		{

		}

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
			if (!string.IsNullOrEmpty(recycle_bin_path))
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

		public void Move(string dest)
		{
			File.Move(saved_full_path, dest);
		}
	}


	internal class FileToManipulate : AbstractFileToManipulate
	{
		public override string saved_full_path
		{
			get { return Path.Combine(MyFileFolder.Photo, saved_path); }
		}
	}

	internal class PendingFileToManipulate: AbstractFileToManipulate
	{
		public override string saved_full_path
		{
			get { return Path.Combine(MyFileFolder.Pending, saved_path); }
		}
	}

}
