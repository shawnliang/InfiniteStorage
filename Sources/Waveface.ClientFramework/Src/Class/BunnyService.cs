using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	class BunnyService : Service
	{
		public BunnyService(IServiceSupplier supplier, string deviceName, string deviceId)
			: base(deviceId, supplier, deviceName)
		{
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "select distinct f.parent_folder " +
								  "from files f, devices d " +
								  "where d.device_id = f.device_id and d.folder_name = @dev and type != 2 " +
								  "order by f.parent_folder desc";

				cmd.Parameters.Add(new SQLiteParameter("@dev", this.Name));

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var dir = reader.GetString(0);
						content.Add(new BunnyContentGroup(new Uri(Path.Combine(BunnyDB.ResourceFolder, Name, dir)), this.ID));
					}
				}
			}

		}
	}
}
