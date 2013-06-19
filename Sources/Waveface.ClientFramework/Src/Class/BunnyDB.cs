using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;

namespace Waveface.ClientFramework
{
	static class BunnyDB
	{
		private static string connString;
		private static string resourceFolder;

		static BunnyDB()
		{
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");
			var dbFilePath = Path.Combine(appDir, "database.s3db");
			connString = string.Format("Data source={0}", dbFilePath);

			resourceFolder = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("BunnyHome").GetValue("ResourceFolder").ToString();
		}

		public static string ConnectionString
		{
			get { return connString; }
		}

		public static SQLiteConnection CreateConnection()
		{
			return new SQLiteConnection(connString);
		}

		public static string ResourceFolder
		{
			get { return resourceFolder; }
		}
	}
}
