using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using System.Data.SQLite;

namespace InfiniteStorage.Share
{
	class ShareEnableTaskDB : IShareEnableTaskDB
	{
		public ICollection<Model.FileAsset> QueryLabelFiles(Model.Label label)
		{
			using (var db = new MyDbContext())
			{
				var q = from lf in db.Object.LabelFiles
						join f in db.Object.Files on lf.file_id equals f.file_id
						join lb in db.Object.Labels on lf.label_id equals lb.label_id
						where !f.deleted && lb.label_id == label.label_id
						select f;

				return q.ToList();
			}
		}

		public void UpdateFileOnCloud(Model.FileAsset file)
		{
			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();

				var cmd = db.CreateCommand();
				cmd.CommandText = "update [Files] set on_cloud = 1 where file_id = @file";
				cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));
				cmd.ExecuteNonQuery();
			}
		}

		public ICollection<Model.ShareRecipient> QueryRecipients(Model.Label label)
		{
			using (var db = new MyDbContext())
			{
				var q = from r in db.Object.ShareRecipients
						where r.label_id == label.label_id
						select r;

				return q.ToList();
			}
		}

		public void UpdateRecipientOnCloud(Model.ShareRecipient recipient)
		{
			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();

				var cmd = db.CreateCommand();
				cmd.CommandText = "update [LabelShareTo] set on_cloud = 1 where id = @id";
				cmd.Parameters.Add(new SQLiteParameter("@id", recipient.id));
				cmd.ExecuteNonQuery();
			}
		}

		public void UpdateShareComplete(Model.Label label)
		{
			using (var db = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				db.Open();

				var cmd = db.CreateCommand();
				cmd.CommandText = "update [Labels] set share_proc_seq = @seq where label_id = @label";
				cmd.Parameters.Add(new SQLiteParameter("@seq", (object)label.seq));
				cmd.Parameters.Add(new SQLiteParameter("@label", label.label_id));
				cmd.ExecuteNonQuery();
			}
		}
	}
}
