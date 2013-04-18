using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using InfiniteStorage.Properties;

namespace InfiniteStorage.Model
{
	public class MyDbContext : IDisposable
	{
		public InfiniteStorageContext Object { get; private set; }
		public static string ConnectionString { get;private set; }
		public static string DbFilePath { get; private set; }

		static MyDbContext()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.ProductName);

			DbFilePath = Path.Combine(appDir, "database.s3db");
			ConnectionString = "Data Source=" + DbFilePath;
		}

		public MyDbContext()
		{
			var conn = new SQLiteConnection(ConnectionString);
			Object = new InfiniteStorageContext(conn, true);
		}

		public void Dispose()
		{
			Object.Dispose();
		}

		public static void InitialzeDatabaseSchema()
		{
			using (var conn = new SQLiteConnection(ConnectionString))
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
[photo_count] INTEGER DEFAULT '0' NULL,
[video_count] INTEGER DEFAULT '0' NULL,
[audio_count] INTEGER DEFAULT '0' NULL
);", conn);
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
[device_id] NVARCHAR(36)  NULL,
[mime_type] NVARCHAR(80)  NULL,
[uti] NVARCHAR(80)  NULL,
[event_time] TIMESTAMP  NULL
);

CREATE INDEX [idx_Files_path_1] ON [Files](
[file_path]  ASC
);", conn);
						cmd.ExecuteNonQuery();

						updateDbSchemaVersion(conn, 2);
						schemaVersion = 2;
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
