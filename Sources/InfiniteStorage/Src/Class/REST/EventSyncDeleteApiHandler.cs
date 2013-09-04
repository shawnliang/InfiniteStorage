using InfiniteStorage.Model;
using System;
using System.Data.SQLite;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class EventSyncDeleteApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("event_ids");

			var event_ids = Parameters["event_ids"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x));

			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "update [Events] set deleted = 1 where event_id = @eventId";
					cmd.Prepare();

					foreach (var evtId in event_ids)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@eventId", evtId);
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
				}
			}
			respondSuccess();
		}
	}
}
