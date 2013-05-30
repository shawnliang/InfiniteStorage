using InfiniteStorage.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace InfiniteStorage.Model
{
	public static class DBInitializer
	{
		public static void InitialzeDatabaseSchema(string connString = null)
		{
			if (connString == null)
				connString = MyDbContext.ConnectionString;

			using (var conn = new SQLiteConnection(connString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				{
					var schemaVersion = getDbSchemaVersion(conn);

					if (schemaVersion == 0L)
					{
						var cmd = new SQLiteCommand(
@"CREATE TABLE [Devices] (
[device_id] NVARCHAR(36)  UNIQUE NULL PRIMARY KEY,
[device_name] NVARCHAR(80)  NULL,
[folder_name] NVARCHAR      NULL
);

", conn);
						cmd.ExecuteNonQuery();

						updateDbSchemaVersion(conn, 1);
						schemaVersion = 1;
					}

					if (schemaVersion == 1L)
					{
						var cmd = new SQLiteCommand(
@"CREATE TABLE [Files] (
[file_id] GUID  NOT NULL PRIMARY KEY,
[file_name] NVARCHAR(100)  NOT NULL,
[file_path] NVARCHAR(1024)  NOT NULL,
[file_size] INTEGER  NULL,
[saved_path] NVARCHAR NULL,
[parent_folder] NVARCHAR NULL,
[device_id] NVARCHAR(36)  NULL,
[type] INTEGER NOT NULL,
[event_time] TIMESTAMP NULL,
[seq] INTEGER NULL,
[deleted] BOOLEAN NULL,
[thumb_ready] BOOLEAN NULL,
[width] INTEGER NULL,
[height] INTEGER NULL
);

CREATE INDEX [idx_Files_path_1] ON [Files](
[file_path]  ASC);

CREATE INDEX [idx_Files_parent_folder_1] ON [Files](
[parent_folder]  ASC);

CREATE INDEX [idx_Files_seq_1] ON [Files](
[seq]  ASC);

", conn);
						cmd.ExecuteNonQuery();

						updateDbSchemaVersion(conn, 2);
						schemaVersion = 2;
					}

					if (schemaVersion == 2L)
					{
						var cmd = new SQLiteCommand(
@"CREATE TABLE [LabelFiles] (
[label_id] GUID  NOT NULL,
[file_id] GUID  NOT NULL,
PRIMARY KEY ([label_id],[file_id])
);

CREATE TABLE [Labels] (
[label_id] GUID  UNIQUE NOT NULL PRIMARY KEY,
[name] NVARCHAR(200)  NULL,
[seq] INTEGER  NOT NULL,
[deleted] BOOLEAN NULL
);

INSERT INTO [Labels] (label_id, name, seq, deleted)
VALUES (@labelId, 'TAG', 1, 0);

", conn);
						cmd.Parameters.Add(new SQLiteParameter("@labelId", Guid.NewGuid()));
						cmd.ExecuteNonQuery();
						updateDbSchemaVersion(conn, 3);
						schemaVersion = 3;
					}

					if (schemaVersion == 3L)
					{
						var cmd = new SQLiteCommand(
@"CREATE TABLE [PendingFiles] (
[file_id] GUID  NOT NULL PRIMARY KEY,
[file_name] NVARCHAR(100)  NOT NULL,
[file_path] NVARCHAR(1024)  NOT NULL,
[file_size] INTEGER  NULL,
[saved_path] NVARCHAR NULL,
[device_id] NVARCHAR(36)  NULL,
[type] INTEGER NOT NULL,
[event_time] TIMESTAMP NULL,
[seq] INTEGER NULL,
[deleted] BOOLEAN NULL,
[thumb_ready] BOOLEAN NULL,
[width] INTEGER NULL,
[height] INTEGER NULL
);


CREATE INDEX [idx_PendingFiles_seq_1] ON [PendingFiles](
[seq]  ASC);

CREATE INDEX [idx_PendingFiles_file_path_1] ON [PendingFiles](
[file_path]  ASC);

", conn);
						cmd.ExecuteNonQuery();

						updateDbSchemaVersion(conn, 4);
						schemaVersion = 4;
					}

					if (schemaVersion == 4L)
					{
						var cmd = new SQLiteCommand(
@"
ALTER TABLE [Labels] Add Column [auto_type] BOOLEAN NULL;
update [Labels] set auto_type = 0;
", conn);
						cmd.ExecuteNonQuery();

						updateDbSchemaVersion(conn, 5);
						schemaVersion = 5;
					}

					if (schemaVersion == 5L)
					{
						var cmd = new SQLiteCommand(
@"
insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@photoToday, 'Today''s Photo', 2, 0, 1);

insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@photoYesterday, 'Yesterday''s Photo', 3, 0, 2);

insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@photoThisWeek, 'This Week''s Photo', 4, 0, 3);

insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@videoToday, 'Today''s video', 3, 0, 4);

insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@videoYesterday, 'Yesterday''s video', 3, 0, 5);

insert into [Labels] (label_id, name, seq, deleted, auto_type)
values (@videoThisWeek, 'This Weeks''s video', 3, 0, 6);

", conn);
						var photoToday = Guid.NewGuid();
						var photoYesterday = Guid.NewGuid();
						var photoThisWeek = Guid.NewGuid();
						var videoToday = Guid.NewGuid();
						var videoYesterday = Guid.NewGuid();
						var videoThisWeek = Guid.NewGuid();

						cmd.Parameters.Add(new SQLiteParameter("@photoToday", photoToday));
						cmd.Parameters.Add(new SQLiteParameter("@photoYesterday", photoYesterday));
						cmd.Parameters.Add(new SQLiteParameter("@photoThisWeek", photoThisWeek));
						cmd.Parameters.Add(new SQLiteParameter("@videoToday", videoToday));
						cmd.Parameters.Add(new SQLiteParameter("@videoYesterday", videoYesterday));
						cmd.Parameters.Add(new SQLiteParameter("@videoThisWeek", videoThisWeek));
						cmd.ExecuteNonQuery();

						Settings.Default.LabelPhotoToday = photoToday;
						Settings.Default.LabelPhotoYesterday = photoYesterday;
						Settings.Default.LabelPhotoThisWeek = photoThisWeek;

						Settings.Default.LabelVideoToday = videoToday;
						Settings.Default.LabelVideoYesterday = videoYesterday;
						Settings.Default.LabelVideoThisWeek = videoThisWeek;

						Settings.Default.Save();

						updateDbSchemaVersion(conn, 6);
						schemaVersion = 6;
					}


					if (schemaVersion == 6L)
					{
						var cmd = new SQLiteCommand(@"select * from PendingFiles", conn);

						var newData = new List<PendingFile>();
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								var filename = reader["file_name"] as string;
								var saved_path = reader["saved_path"] as string;
								var file_id = (Guid)reader["file_id"];

								try
								{
									var src = Path.Combine(MyFileFolder.Photo, ".pending", saved_path);
									var newSavedPath = saved_path + Path.GetExtension(filename) + "_";
									var dest = Path.Combine(MyFileFolder.Photo, ".pending", newSavedPath);

									File.Move(src, dest);

									newData.Add(new PendingFile { file_id = file_id, saved_path = newSavedPath });
								}
								catch (Exception err)
								{
									log4net.LogManager.GetLogger("migrate").Warn("unable to migrate pending file", err);
								}
							}
						}

						var update = new SQLiteCommand(@"update [PendingFiles] set saved_path = @path where file_id = @file_id", conn);
						update.Prepare();

						foreach (var newd in newData)
						{
							update.Parameters.Clear();
							update.Parameters.Add(new SQLiteParameter("@file_id", newd.file_id));
							update.Parameters.Add(new SQLiteParameter("@path", newd.saved_path));

							update.ExecuteNonQuery();
						}

						updateDbSchemaVersion(conn, 7);
						schemaVersion = 7;
					}

					transaction.Commit();
				}

			}
		}


		private static long getDbSchemaVersion(SQLiteConnection conn)
		{
			var cmd = new SQLiteCommand("PRAGMA user_version;", conn);
			return (long)cmd.ExecuteScalar();
		}

		private static int updateDbSchemaVersion(SQLiteConnection conn, int version)
		{
			var cmd = new SQLiteCommand(string.Format("PRAGMA user_version={0};", version), conn);
			return cmd.ExecuteNonQuery();
		}

	}
}
