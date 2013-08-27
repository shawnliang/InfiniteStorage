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
		public const string BULK_INSERT_QUEUE = "BulkInsertQueue";
		public const string BULK_INSERT_QUEUE_CS = "BulkInsertQueueCS";
		public const string BULK_INSERT_LAST_FLUSH_TIME = "BulkInsertQueue_LastFlushTime";
		public const int BULK_INSERT_BATCH_SIZE = 100;
		public const int BULK_INSERT_BATCH_SECONDS = 2;

		public void SaveFileRecord(Model.FileAsset file)
		{
			SaveFileRecords(new List<FileAsset> { file });
		}

		public void SaveFileRecords(List<Model.FileAsset> files)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "INSERT OR REPLACE INTO [Files] (file_id, file_name, file_path, file_size, saved_path, parent_folder, device_id, type, event_time, seq, deleted, thumb_ready, on_cloud, width, height, has_origin, import_time) values (" +
					 "@fid, @fname, @fpath, @fsize, @saved_path, @parent_folder, @devid, @type, @time, @seq, @del, @thumb, @oncloud, @width, @height, @has_origin, @import_time)";
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
						cmd.Parameters.Add(new SQLiteParameter("@import_time", file.import_time));

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


		public Guid? QueryFileId(string device_id, string file_path, ProtocolContext ctx)
		{
			var cs = ctx.GetData(BULK_INSERT_QUEUE_CS);
			lock (cs)
			{
				using (var db = new MyDbContext())
				{
					var file = from f in db.Object.Files
							   where f.file_path.Equals(file_path, StringComparison.InvariantCultureIgnoreCase) && f.device_id == device_id
							   select new { file_id = f.file_id };

					if (file.Any())
						return file.First().file_id;



					if (ctx.ContainsData(BULK_INSERT_QUEUE))
					{
						var queue = ctx.GetData(BULK_INSERT_QUEUE) as List<FileAsset>;
						var qfile = queue.Where(x => x.file_path.Equals(file_path, StringComparison.InvariantCultureIgnoreCase) && x.device_id == device_id);

						if (qfile.Any())
							return qfile.First().file_id;
					}

					return (Guid?)null;
				}
			}
		}

		public void SaveFileRecord(FileAsset file, ProtocolContext ctx)
		{
			var cs = ctx.GetData(BULK_INSERT_QUEUE_CS);
			lock (cs)
			{
				List<FileAsset> queue = null;
				DateTime lastFlushTime;
				if (ctx.ContainsData(BULK_INSERT_QUEUE))
				{
					queue = ctx.GetData(BULK_INSERT_QUEUE) as List<FileAsset>;
					lastFlushTime = (DateTime)ctx.GetData(BULK_INSERT_LAST_FLUSH_TIME);
				}
				else
				{
					queue = new List<FileAsset>();
					lastFlushTime = DateTime.Now;
					ctx.SetData(BULK_INSERT_QUEUE, queue);
					ctx.SetData(BULK_INSERT_LAST_FLUSH_TIME, lastFlushTime);
				}

				queue.Add(file);

				if (queue.Count > BULK_INSERT_BATCH_SIZE || DateTime.Now - lastFlushTime > TimeSpan.FromSeconds(BULK_INSERT_BATCH_SECONDS))
				{
					flushFileRecords_noLock(ctx);
				}
			}
		}

		public void FlushFileRecords(ProtocolContext ctx)
		{
			var cs = ctx.GetData(BULK_INSERT_QUEUE_CS);
			lock (cs)
			{
				flushFileRecords_noLock(ctx);
			}
		}

		private void flushFileRecords_noLock(ProtocolContext ctx)
		{
			if (ctx.ContainsData(BULK_INSERT_QUEUE))
			{
				var queue = (List<FileAsset>)ctx.GetData(BULK_INSERT_QUEUE);
				this.SaveFileRecords(queue);

				queue.Clear();
				ctx.SetData(BULK_INSERT_LAST_FLUSH_TIME, DateTime.Now);
			}
		}

		public void FlushFileRecordsIfNoFlushedForXSec(int sec, ProtocolContext ctx)
		{
			var cs = ctx.GetData(BULK_INSERT_QUEUE_CS);
			lock (cs)
			{
				if (ctx.ContainsData(BULK_INSERT_QUEUE))
				{
					var lastFlushTime = (DateTime)ctx.GetData(BULK_INSERT_LAST_FLUSH_TIME);

					var noFlushPeriod = DateTime.Now - lastFlushTime;

					if (noFlushPeriod > TimeSpan.FromSeconds(BULK_INSERT_BATCH_SECONDS * 2))
					{
						flushFileRecords_noLock(ctx);
					}
				}
			}
		}
	}
}
