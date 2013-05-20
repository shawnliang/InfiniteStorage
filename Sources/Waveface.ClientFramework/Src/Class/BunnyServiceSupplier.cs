using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyServiceSupplier : ServiceSupplier
	{
		#region Static Var
		private static BunnyServiceSupplier _default;
		#endregion


		#region Public Static Property
		public static BunnyServiceSupplier Default
		{
			get
			{
				return _default ?? (_default = new BunnyServiceSupplier());
			}
		}
		#endregion


		#region Protected Property
		protected override ServiceSupplierInfo m_Info
		{
			get { throw new NotImplementedException(); }
		}
		#endregion


		#region Public Property
		public override string ID
		{
			get { throw new NotImplementedException(); }
		}
		#endregion


		#region Constructor
		private BunnyServiceSupplier()
		{
			GetServices();
		}
		#endregion


		#region Private Method
		private void GetServices()
		{
			var services = new List<Service>();
			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT * FROM Devices", conn);

			var dr = cmd.ExecuteReader();

			while (dr.Read())
			{
				var deviceName = dr["device_name"].ToString();
				var folderName = dr["folder_name"].ToString();
				services.Add(new Service(this, deviceName, (contents) =>
				{
					var resourceFolderValue = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("BunnyHome").GetValue("ResourceFolder").ToString();
					var resourcePath = Path.Combine(resourceFolderValue, folderName);

					var directories = Directory.GetDirectories(resourcePath);

					foreach (var directory in directories)
					{
						contents.Add(new BunnyContentGroup(new Uri(directory)));
					}
				}));
			}

			conn.Close();

			this.Services = services;
		}
		#endregion
	}
}
