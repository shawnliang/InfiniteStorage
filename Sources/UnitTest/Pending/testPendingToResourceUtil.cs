using InfiniteStorage;
using InfiniteStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
namespace UnitTest.Pending
{
	[Ignore]
	[TestClass]
	public class testPendingToResourceUtil
	{
		const string ConnString = "Data Source=db.s3db";

		List<Guid> file_ids = new List<Guid>();
		string dev_id = Guid.NewGuid().ToString();

		[TestInitialize]
		public void setup()
		{
			if (File.Exists("db.s3db"))
				File.Delete("db.s3db");

			file_ids.Clear();
			file_ids.Add(Guid.NewGuid());
			file_ids.Add(Guid.NewGuid());
			file_ids.Add(Guid.NewGuid());

			file_ids.Sort();

			DBInitializer.InitialzeDatabaseSchema(ConnString);

			using (var db = new SQLiteConnection(ConnString))
			{
				db.Open();
				using (var cmd = db.CreateCommand())
				{
					cmd.CommandText =
					   "insert into pendingFiles (file_id, file_name, file_path, file_size, saved_path, device_id, type, event_time, seq, deleted, thumb_ready, width, height) " +
					   "values (@id, @name, @path, @size, @saved_path, @dev_id, 0, @time, @seq, 0, 1, 1024, 768)";
					cmd.Connection = db;

					int i = 0;
					foreach (var fid in file_ids)
					{
						cmd.Parameters.Add(new SQLiteParameter("@id", fid));
						cmd.Parameters.Add(new SQLiteParameter("@name", "name_" + i));
						cmd.Parameters.Add(new SQLiteParameter("@path", "path_" + i));
						cmd.Parameters.Add(new SQLiteParameter("@size", i + 1000));
						cmd.Parameters.Add(new SQLiteParameter("@saved_path", "saved_" + i));
						cmd.Parameters.Add(new SQLiteParameter("@dev_id", dev_id));
						cmd.Parameters.Add(new SQLiteParameter("@time", DateTime.Now));
						cmd.Parameters.Add(new SQLiteParameter("@seq", i));

						i++;
						cmd.ExecuteNonQuery();
					}

					using (var cmd2 = db.CreateCommand())
					{
						cmd2.Connection = db;
						cmd2.CommandText = "insert into Devices (device_id, device_name, folder_name) values ('" + dev_id + "', 'dev', 'devfolder')";
						cmd2.ExecuteNonQuery();
					}
				}
			}
		}

		[TestMethod]
		public void GetPendingFiles()
		{
			var util = new PendingToResourceUtil(ConnString, "dev", "res");

			var fids = file_ids.Take(2).ToList();
			var result = util.GetPendingFiles(fids).OrderBy(x => x.file_id).ToList();

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(fids[0], result[0].file_id);
			Assert.AreEqual("name_0", result[0].file_name);
			Assert.AreEqual("saved_0", result[0].saved_path);
			Assert.AreEqual("devfolder", result[0].dev_folder);

			Assert.AreEqual(fids[1], result[1].file_id);
			Assert.AreEqual("name_1", result[1].file_name);
			Assert.AreEqual("saved_1", result[1].saved_path);
			Assert.AreEqual("devfolder", result[1].dev_folder);
		}

		[TestMethod]
		public void MoveDbRecords()
		{
			var util = new PendingToResourceUtil(ConnString, "dev", "res");

			util.MoveDbRecord(
				new List<FileData>{
					new FileData{ file_id = file_ids[0], saved_path = "newp0", parent_folder = "parent0"},
					new FileData{ file_id = file_ids[1], saved_path = "newp1", parent_folder = "parent1"},
				});


			///// verify
			using (var conn = new SQLiteConnection(ConnString))
			{
				conn.Open();

				int i = 0;
				foreach (var fid in new Guid[] { file_ids[0], file_ids[1] })
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "select * from Files where file_id = @id";
						cmd.Parameters.Add(new SQLiteParameter("@id", fid));
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								Assert.AreEqual("name_" + i, reader["file_name"]);
								Assert.AreEqual("path_" + i, reader["file_path"]);
								Assert.AreEqual(1000L + i, reader["file_size"]);
								Assert.AreEqual("newp" + i, reader["saved_path"]);
								Assert.AreEqual("parent" + i, reader["parent_folder"]);
								Assert.AreEqual(dev_id, reader["device_id"]);
								Assert.AreEqual(0L, reader["type"]);
								Assert.AreEqual(1024L, reader["width"]);
								Assert.AreEqual(768L, reader["height"]);
							}
						}
					}

					using (var checkCmd = conn.CreateCommand())
					{
						checkCmd.CommandText = "select 1 from PendingFiles where file_id= @id";
						checkCmd.Parameters.Add(new SQLiteParameter("@id", file_ids[0]));
						Assert.AreEqual(null, checkCmd.ExecuteScalar());
					}
					i++;
				} // foreach
			}

		}
	}
}
