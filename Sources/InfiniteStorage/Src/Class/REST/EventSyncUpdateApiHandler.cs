using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using Newtonsoft.Json;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class EventSyncUpdateApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("events");

			var syncData = JsonConvert.DeserializeObject<EventSyncData>(Parameters["events"]);

			writeSyncDataToDb(syncData);

			respondSuccess();
		}

		private void writeSyncDataToDb(EventSyncData syncData)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();
				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				using (var del = conn.CreateCommand())
				using (var ins = conn.CreateCommand())
				{
					cmd.CommandText = "insert or replace into [Events] (event_id, content, start, end, short_address, cover, deleted, device_id) " +
									  "values (@eventId, @content, @start, @end, @addr, @cover, 0, @deviceId)";
					cmd.Prepare();

					del.CommandText = "delete from [EventFiles] where event_id = @eventId";
					del.Prepare();

					ins.CommandText = "insert into [EventFiles] (event_id, file_id) values (@eventId, @fileId)";
					ins.Prepare();

					foreach (var data in syncData.events)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@eventId", data.event_id);
						cmd.Parameters.AddWithValue("@content", data.content);
						cmd.Parameters.AddWithValue("@start", data.start);
						cmd.Parameters.AddWithValue("@end", data.end);
						cmd.Parameters.AddWithValue("@addr", data.short_address);
						cmd.Parameters.AddWithValue("@cover", data.cover);
						cmd.Parameters.AddWithValue("@deviceId", syncData.device_id);
						cmd.ExecuteNonQuery();

						del.Parameters.Clear();
						del.Parameters.AddWithValue("@eventId", data.event_id);
						del.ExecuteNonQuery();

						foreach(var file in data.files)
						{
							ins.Parameters.Clear();
							ins.Parameters.AddWithValue("@eventId", data.event_id);
							ins.Parameters.AddWithValue("@fileId", file);
							ins.ExecuteNonQuery();
						}
					}

					transaction.Commit();
				}
			}
		}
	}


	class EventData
	{
		public Guid event_id { get; set; }
		public string content { get; set; }
		public DateTime start { get; set; }
		public DateTime end { get; set; }
		public string short_address { get; set; }
		public Guid? cover { get; set; }
		public List<Guid> files { get; set; }
	}

	class EventSyncData
	{
		public string device_id { get; set; }

		public List<EventData> events { get; set; }
	}
}
