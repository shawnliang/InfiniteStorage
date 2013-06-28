using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;
using System.Data.SQLite;

namespace InfiniteStorage.REST
{
	class LabelTagApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("file_id", "label_id");

			var file_ids = Parameters["file_id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x)).ToList();
			var label_id = new Guid(Parameters["label_id"]);


			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var query = conn.CreateCommand())
				using (var insert = conn.CreateCommand())
				{
					query.CommandText = "select 1 from [LabelFiles] where label_id = @label and file_id = @file";
					query.Prepare();

					insert.CommandText = "insert into [LabelFiles] (label_id, file_id) values (@label, @file)";
					insert.Prepare();


					using (var trans = conn.BeginTransaction())
					{
						bool inserted = false;

						foreach (var file_id in file_ids)
						{
							var param_file = new SQLiteParameter("@file", file_id);
							var param_label = new SQLiteParameter("@label", label_id);
							query.Parameters.Clear();
							query.Parameters.Add(param_file);
							query.Parameters.Add(param_label);

							var alreadyTagged = (query.ExecuteScalar() != null);

							if (alreadyTagged)
								continue;


							insert.Parameters.Clear();
							insert.Parameters.Add(param_file);
							insert.Parameters.Add(param_label);
							insert.ExecuteNonQuery();
							inserted = true;
						}

						if (inserted)
						{
							using (var update = conn.CreateCommand())
							{
								update.CommandText = "update [Labels] set seq = @seq where label_id = @label";
								update.Parameters.Add(new SQLiteParameter("@label", label_id));
								update.Parameters.Add(new SQLiteParameter("@seq", (object)SeqNum.GetNextSeq()));
								update.ExecuteNonQuery();
							}
						}

						trans.Commit();
					}
				}
			}

			respondSuccess();
		}
	}
}
