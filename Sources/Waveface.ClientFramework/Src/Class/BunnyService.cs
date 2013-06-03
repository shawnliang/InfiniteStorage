using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	class BunnyService : Service
	{
		public BunnyService(IServiceSupplier supplier, string devFolderName, string deviceId)
			: base(deviceId, supplier, devFolderName)
		{
			SetContents(PopulateContent);
		}

		private void PopulateContent(ObservableCollection<IContentEntity> content)
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				var cmd = conn.CreateCommand();
				cmd.CommandText = "select [name] from [Folders] " +
								  "where parent_folder = @parent " +
								  "order by name desc";


				cmd.Parameters.Add(new SQLiteParameter("@parent", this.Name));

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var dir = reader.GetString(0);
						content.Add(new BunnyContentGroup(Name, dir, this.ID));
					}
				}
			}

		}
	}
}
