using System;
using System.Data.SQLite;
using System.IO;

namespace InfiniteStorage.Model
{
	public class MyDbContext : IDisposable
	{
		public InfiniteStorageContext Object { get; private set; }
		public static string ConnectionString { get; private set; }
		public static string DbFilePath { get; private set; }

		static MyDbContext()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

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
	}
}
