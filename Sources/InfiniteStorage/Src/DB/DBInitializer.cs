using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using InfiniteStorage.Properties;

namespace InfiniteStorage.Model
{
	static class DBInitializer
	{
		public static void InitialzeDatabaseSchema()
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
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
