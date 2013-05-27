using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Waveface.Model;
using System.Data.SQLite;

namespace Waveface.ClientFramework
{
	public class BunnyContentGroup : ContentGroup
	{
		public string DeviceId { get; private set; }

		#region Constructor
		public BunnyContentGroup()
		{

		}

		public BunnyContentGroup(Uri uri, string deviceId)
			: base(uri.LocalPath.GetHashCode().ToString(), Path.GetFileName(uri.LocalPath), uri)
		{

			this.DeviceId = deviceId;

			SetContents((contents) =>
			{
				using (var conn = BunnyDB.CreateConnection())
				{
					conn.Open();

					var cmd = conn.CreateCommand();
					cmd.CommandText =
						"select file_id, file_name from Files " +
						"where parent_folder = @parent and device_id = @dev " +
						"order by event_time";

					cmd.Parameters.Add(new SQLiteParameter("@parent", this.Name));
					cmd.Parameters.Add(new SQLiteParameter("@dev", DeviceId));

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var file_path = Path.Combine(Uri.LocalPath, reader["file_name"].ToString());
							contents.Add(new BunnyContent(new Uri(file_path), reader["file_id"].ToString()));
						}
					}

				}
			});
		}
		#endregion
	}
}
