using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using InfiniteStorage.WebsocketProtocol;
using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteStorage
{
	class AutoLabelController
	{
		public void FileReceived(object sender, WebsocketEventArgs args)
		{
			try
			{
				var file = args.ctx.fileCtx;


				if (file.type == FileAssetType.image)
				{
					if (withinToday(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelPhotoToday };

						updateLabels(args, labels);
					}

					if (withinYesterday(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelPhotoYesterday };
						updateLabels(args, labels);
					}

					if (withinThisWeek(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelPhotoThisWeek };

						updateLabels(args, labels);
					}
				}
				else if (file.type == FileAssetType.video)
				{
					if (withinToday(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelVideoToday };

						updateLabels(args, labels);
					}

					if (withinYesterday(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelVideoYesterday };
						updateLabels(args, labels);
					}

					if (withinThisWeek(file.datetime))
					{
						var labels = new Guid[] { Settings.Default.LabelVideoThisWeek };

						updateLabels(args, labels);
					}
				}
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("unable to auto label on file: " + args.ctx.fileCtx.file_id, e);
			}
		}

		private bool withinYesterday(DateTime dateTime)
		{
			var yesterday = DateTime.Now.AddDays(-1.0);

			return dateTime.Year == yesterday.Year && dateTime.Month == yesterday.Month && dateTime.Day == yesterday.Day;
		}

		private bool withinThisWeek(DateTime dateTime)
		{
			var now = DateTime.Now;
			var startOfThisWeek = now.AddDays(-6.0).TrimToDay();
			var tomorrow = now.AddDays(1.0).TrimToDay();

			return startOfThisWeek <= dateTime && dateTime < tomorrow;
		}

		private bool withinToday(DateTime dateTime)
		{
			var now = DateTime.Now;

			return now.Year == dateTime.Year && now.Month == dateTime.Month && now.Day == dateTime.Day;
		}

		private void updateLabels(WebsocketEventArgs args, Guid[] labels)
		{
			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();
				using (var transaction = db.BeginTransaction())
				{

					tagLabelsOnFile(args, db, labels);
					updateLabelSeq(db, labels);

					transaction.Commit();
				}
			}
		}

		private static void updateLabelSeq(SQLiteConnection db, IEnumerable<Guid> labels)
		{
			var cmd = db.CreateCommand();
			cmd.CommandText = "update [labels] set seq = @seq where label_id = @label_id";
			cmd.Prepare();

			foreach (var label in labels)
			{
				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SQLiteParameter("@seq", SeqNum.GetNextSeq()));
				cmd.Parameters.Add(new SQLiteParameter("@label_id", label));

				cmd.ExecuteNonQuery();
			}
		}

		private static void tagLabelsOnFile(WebsocketEventArgs args, SQLiteConnection db, Guid[] labels)
		{
			var cmd = db.CreateCommand();
			cmd.CommandText = "insert into [labelFiles] (label_id, file_id) values (@lid, @fid)";
			cmd.Prepare();

			foreach (var label_id in labels)
			{
				cmd.Parameters.Clear();
				cmd.Parameters.Add(new SQLiteParameter("@lid", label_id));
				cmd.Parameters.Add(new SQLiteParameter("@fid", args.ctx.fileCtx.file_id));
				cmd.ExecuteNonQuery();
			}

		}

		public void RemoveOutOfDateAutoLabels()
		{
			var toRemove = getOutOfDateLabeledFiles();

			if (toRemove.Any())
				unlinkLabeledFiles(toRemove);

			recomputeYesterday();
		}

		private void recomputeYesterday()
		{
			recomputeYesterday(FileAssetType.image);
			recomputeYesterday(FileAssetType.audio);
		}

		private static void recomputeYesterday(FileAssetType file_type)
		{
			var label_id = (file_type == FileAssetType.image) ? Settings.Default.LabelPhotoYesterday : Settings.Default.LabelVideoYesterday;

			var yes_start = DateTime.Now.AddDays(-1.0).TrimToDay();
			var yes_end = DateTime.Now.TrimToDay();


			using (var db = new MyDbContext())
			{
				var actual = (from f in db.Object.Files
							  where f.event_time >= yes_start && f.event_time < yes_end && f.type == (int)file_type
							  select f.file_id).Union(
							  from f in db.Object.PendingFiles
							  where f.event_time >= yes_start && f.event_time < yes_end && f.type == (int)file_type
							  select f.file_id
							  ).ToList();

				var current = (from f in db.Object.LabelFiles
							   where f.label_id == label_id
							   select f).ToList();

				var areCurrentAndActualIdentical = actual.Count == current.Count && actual.TrueForAll(x => current.Where(y => y.file_id == x).Any());

				if (!areCurrentAndActualIdentical)
				{
					foreach (var cur in current)
					{
						db.Object.LabelFiles.Remove(cur);
					}

					foreach (var act in actual)
					{
						db.Object.LabelFiles.Add(new LabeledFile { file_id = act, label_id = label_id });
					}

					var lbRecord = (from lb in db.Object.Labels
									where lb.label_id == label_id
									select lb).First();
					lbRecord.seq = SeqNum.GetNextSeq();

					db.Object.SaveChanges();
				}
			}
		}

		private static List<Guid> computeLabeledFiles(SQLiteConnection conn, DateTime yesterdayStart, DateTime yesterdayEnd, FileAssetType type)
		{
			var fileIds = new List<Guid>();

			var cmd = conn.CreateCommand();
			cmd.CommandText =
				"select file_id from Files        where event_time >= @yes_start and event_time < @today and [type] = @type union " +
				"select file_id from PendingFiles where event_time >= @yes_start and event_time < @today and [type] = @type";
			cmd.Parameters.Add(new SQLiteParameter("@yes_start", yesterdayStart));
			cmd.Parameters.Add(new SQLiteParameter("@today", yesterdayEnd));
			cmd.Parameters.Add(new SQLiteParameter("@type", (object)(int)type));
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					fileIds.Add(reader.GetGuid(0));
				}
			}
			return fileIds;
		}

		private static void unlinkLabeledFiles(List<LabeledFile> toRemove)
		{
			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();
				using (var transaction = db.BeginTransaction())
				{
					var cmd = db.CreateCommand();
					cmd.CommandText = "delete from labelFiles where label_id=@label and file_id=@file";
					cmd.Prepare();

					foreach (var file in toRemove)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("@label", file.label_id));
						cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));
						cmd.ExecuteNonQuery();
					}

					var changedLabels = toRemove.Select(x => x.label_id).Distinct();
					
					updateLabelSeq(db, changedLabels);

					transaction.Commit();
				}
			}
		}

		private List<LabeledFile> getOutOfDateLabeledFiles()
		{
			var toRemove = new List<LabeledFile>();

			using (var db = new MyDbContext())
			{
				var labelFiles = (from f in db.Object.LabelFiles
								  join l in db.Object.Labels on f.label_id equals l.label_id
								  join t in db.Object.Files on f.file_id equals t.file_id
								  where l.auto_type > (int)AutoLabelType.NotAuto
								  select
								  new
								  {
									  file_id = f.file_id,
									  label_id = f.label_id,
									  auto_type = l.auto_type,
									  taken_time = t.event_time
								  }).Union(
								 from f in db.Object.LabelFiles
								 join l in db.Object.Labels on f.label_id equals l.label_id
								 join t in db.Object.PendingFiles on f.file_id equals t.file_id
								 where l.auto_type > (int)AutoLabelType.NotAuto
								 select
								 new
								 {
									 file_id = f.file_id,
									 label_id = f.label_id,
									 auto_type = l.auto_type,
									 taken_time = t.event_time
								 }
								 );

				foreach (var labelFile in labelFiles)
				{
					switch (labelFile.auto_type)
					{
						case (int)AutoLabelType.PhotoThisWeek:
						case (int)AutoLabelType.VideoThisWeek:
							if (!withinThisWeek(labelFile.taken_time))
								toRemove.Add(new LabeledFile { label_id = labelFile.label_id, file_id = labelFile.file_id });
							break;

						case (int)AutoLabelType.PhotoYesterday:
						case (int)AutoLabelType.VideoYesterday:
							if (!withinYesterday(labelFile.taken_time))
								toRemove.Add(new LabeledFile { label_id = labelFile.label_id, file_id = labelFile.file_id });
							break;

						case (int)AutoLabelType.PhotoToday:
						case (int)AutoLabelType.VideoToday:
							if (!withinToday(labelFile.taken_time))
								toRemove.Add(new LabeledFile { label_id = labelFile.label_id, file_id = labelFile.file_id });
							break;
					}
				}
			}
			return toRemove;
		}


	}

}
