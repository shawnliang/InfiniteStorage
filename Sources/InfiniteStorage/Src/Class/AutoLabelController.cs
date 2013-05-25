using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.WebsocketProtocol;
using InfiniteStorage.Model;
using System.Data.SQLite;
using InfiniteStorage.Properties;

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

		private void updateLabelSeq(SQLiteConnection db, Guid[] labels)
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
	}
}
