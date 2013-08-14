using InfiniteStorage.Model;
using System;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using System.Collections.Generic;

namespace InfiniteStorage.WebsocketProtocol
{
	class TransmitUtility : ITransmitStateUtility, IFileUtility
	{
		public void SaveFileRecord(Model.FileAsset file)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "INSERT OR REPLACE INTO [Files] (file_id, file_name, file_path, file_size, saved_path, parent_folder, device_id, type, event_time, seq, deleted, thumb_ready, on_cloud, width, height, has_origin) values (" +
					 "@fid, @fname, @fpath, @fsize, @saved_path, @parent_folder, @devid, @type, @time, @seq, @del, @thumb, @oncloud, @width, @height, @has_origin)";
					cmd.Parameters.Add(new SQLiteParameter("@fid", file.file_id));
					cmd.Parameters.Add(new SQLiteParameter("@fname", file.file_name));
					cmd.Parameters.Add(new SQLiteParameter("@fpath", file.file_path));
					cmd.Parameters.Add(new SQLiteParameter("@fsize", file.file_size));
					cmd.Parameters.Add(new SQLiteParameter("@saved_path", file.saved_path));
					cmd.Parameters.Add(new SQLiteParameter("@parent_folder", file.parent_folder));
					cmd.Parameters.Add(new SQLiteParameter("@devid", file.device_id));
					cmd.Parameters.Add(new SQLiteParameter("@type", file.type));
					cmd.Parameters.Add(new SQLiteParameter("@time", file.event_time));
					cmd.Parameters.Add(new SQLiteParameter("@seq", file.seq));
					cmd.Parameters.Add(new SQLiteParameter("@del", file.deleted));
					cmd.Parameters.Add(new SQLiteParameter("@thumb", file.thumb_ready));
					cmd.Parameters.Add(new SQLiteParameter("@oncloud", file.on_cloud == true));
					cmd.Parameters.Add(new SQLiteParameter("@width", file.width));
					cmd.Parameters.Add(new SQLiteParameter("@height", file.height));
					cmd.Parameters.Add(new SQLiteParameter("@has_origin", file.has_origin));

					cmd.ExecuteNonQuery();
				}

			}
		}

		public void SaveFileRecords(List<Model.FileAsset> files)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "INSERT OR REPLACE INTO [Files] (file_id, file_name, file_path, file_size, saved_path, parent_folder, device_id, type, event_time, seq, deleted, thumb_ready, on_cloud, width, height, has_origin) values (" +
					 "@fid, @fname, @fpath, @fsize, @saved_path, @parent_folder, @devid, @type, @time, @seq, @del, @thumb, @oncloud, @width, @height, @has_origin)";
					cmd.Prepare();

					foreach (var file in files)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@fid", file.file_id));
						cmd.Parameters.Add(new SQLiteParameter("@fname", file.file_name));
						cmd.Parameters.Add(new SQLiteParameter("@fpath", file.file_path));
						cmd.Parameters.Add(new SQLiteParameter("@fsize", file.file_size));
						cmd.Parameters.Add(new SQLiteParameter("@saved_path", file.saved_path));
						cmd.Parameters.Add(new SQLiteParameter("@parent_folder", file.parent_folder));
						cmd.Parameters.Add(new SQLiteParameter("@devid", file.device_id));
						cmd.Parameters.Add(new SQLiteParameter("@type", file.type));
						cmd.Parameters.Add(new SQLiteParameter("@time", file.event_time));
						cmd.Parameters.Add(new SQLiteParameter("@seq", file.seq));
						cmd.Parameters.Add(new SQLiteParameter("@del", file.deleted));
						cmd.Parameters.Add(new SQLiteParameter("@thumb", file.thumb_ready));
						cmd.Parameters.Add(new SQLiteParameter("@oncloud", file.on_cloud == true));
						cmd.Parameters.Add(new SQLiteParameter("@width", file.width));
						cmd.Parameters.Add(new SQLiteParameter("@height", file.height));
						cmd.Parameters.Add(new SQLiteParameter("@has_origin", file.has_origin));

						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
		}


		public bool HasDuplicateFile(FileContext file, string device_id)
		{
			var full_path = Path.Combine(file.folder, file.file_name);

			using (var db = new MyDbContext())
			{
				var saved_file = from f in db.Object.Files
								 where f.file_path.Equals(full_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
								 select new { 
									has_origin = f.has_origin
								 };

				if (file.is_thumbnail)
					return saved_file.Any();
				else
					return saved_file.Any() && saved_file.First().has_origin;
			}
		}

		public long GetNextSeq()
		{
			return SeqNum.GetNextSeq();
		}


		public Guid? QueryFileId(string device_id, string file_path)
		{
			using (var db = new MyDbContext())
			{
				var file = from f in db.Object.Files
							  where f.file_path.Equals(file_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
							  select new { file_id = f.file_id };

				return file.Any() ? file.First().file_id : (Guid?)null;
			}
		}
	}
}
