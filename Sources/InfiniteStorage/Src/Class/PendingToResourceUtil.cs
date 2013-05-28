using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace InfiniteStorage
{
	public class PendingToResourceUtil : IPendingToResourceUtil
	{
		private string connString;
		private string resource_dir;
		private string dev_path;
		private FileMover mover = new FileMover();

		public PendingToResourceUtil(string connString, string dev_folder, string res_folder = null)
		{
			this.connString = connString;
			this.resource_dir = res_folder ?? MyFileFolder.Photo;
			this.dev_path = Path.Combine(resource_dir, dev_folder);
		}

		public string CreateFolder(string folderUnderDevFolder)
		{
			var dir = Path.Combine(dev_path, folderUnderDevFolder);

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			return folderUnderDevFolder;
		}

		public List<PendingFileData> GetPendingFiles(List<Guid> file_ids)
		{
			var ret = new List<PendingFileData>();

			using (var conn = new SQLiteConnection(connString))
			{
				conn.Open();
				using (var transaction = conn.BeginTransaction())
				{
					var cmd = conn.CreateCommand();


					var i = 0;
					var pars = new List<string>();
					foreach (var fid in file_ids)
					{
						cmd.Parameters.Add(new SQLiteParameter("@fid" + i, fid));
						pars.Add("@fid" + i);
						i++;
					}

					var inClause = "(" + string.Join(",", pars.ToArray()) + ")";

					cmd.CommandType = CommandType.Text;
					cmd.CommandText =
						"select f.file_name, f.saved_path, d.folder_name, f.file_id from PendingFiles f, devices d " +
						"where f.device_id = d.device_id and f.file_id in " + inClause;
					cmd.Connection = conn;
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var data = new PendingFileData
							{
								file_name = reader.GetString(0),
								saved_path = reader.GetString(1),
								dev_folder = reader.GetString(2),
								file_id = reader.GetGuid(3)
							};

							ret.Add(data);
						}
					}

					transaction.Commit();
				}
			}

			return ret;
		}

		public string Move(string from, string to)
		{
			return mover.Move(from, to);
		}

		public void MoveDbRecord(List<FileData> data)
		{
			using (var db = new SQLiteConnection(connString))
			{
				db.Open();

				using (var transaction = db.BeginTransaction())
				{
					copyToFilesTable(data, db);

					deletePendingFileRecords(data, db);

					transaction.Commit();
				}
			}
		}

		private static void deletePendingFileRecords(List<FileData> data, SQLiteConnection db)
		{
			var delCmd = db.CreateCommand();
			delCmd.CommandText =
				"delete from PendingFiles where file_id = @fid";
			delCmd.Prepare();

			foreach (var file in data)
			{
				delCmd.Parameters.Clear();
				delCmd.Parameters.Add(new SQLiteParameter("@fid", file.file_id));
				delCmd.ExecuteNonQuery();
			}
		}

		private static void copyToFilesTable(List<FileData> data, SQLiteConnection db)
		{
			var cmd = db.CreateCommand();
			cmd.CommandText =
				"insert into Files (file_id, file_name, file_path, file_size, saved_path, parent_folder, device_id, type, event_time, seq, deleted, thumb_ready, width, height) " +
				"select file_id, file_name, file_path, file_size, @saved, @parent, device_id, type, event_time, @seq, deleted, thumb_ready, width, height from [PendingFiles] " +
				"where file_id = @fid";
			cmd.Prepare();

			foreach (var file in data)
			{
				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SQLiteParameter("@saved", file.saved_path));
				cmd.Parameters.Add(new SQLiteParameter("@parent", file.parent_folder));
				cmd.Parameters.Add(new SQLiteParameter("@fid", file.file_id));
				cmd.Parameters.Add(new SQLiteParameter("@seq", SeqNum.GetNextSeq()));
				cmd.ExecuteNonQuery();
			}
		}

		public string GetResourceFolder()
		{
			return resource_dir;
		}

		public string GetPendingFolder()
		{
			return MyFileFolder.Pending;
		}
	}
}
