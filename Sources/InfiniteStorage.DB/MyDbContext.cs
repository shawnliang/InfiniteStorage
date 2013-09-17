#region

using System;
using System.Data.SQLite;
using System.IO;

#endregion

namespace InfiniteStorage.Model
{
	public class MyDbContext : IDisposable
	{
		public InfiniteStorageContext Object { get; private set; }
		public static string ConnectionString { get; private set; }
		public static string DbFilePath { get; private set; }

		private SQLiteConnection conn;

		static MyDbContext()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			DbFilePath = Path.Combine(appDir, "database.s3db");

			ConnectionString = "Data Source=" + DbFilePath;
		}

		public MyDbContext()
		{
			conn = new SQLiteConnection(ConnectionString);
			Object = new InfiniteStorageContext(conn, true);
		}

		public void Dispose()
		{
			Object.Dispose();
			conn.Dispose();
		}
	}
}