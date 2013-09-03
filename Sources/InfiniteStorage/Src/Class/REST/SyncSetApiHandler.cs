using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class SyncSetApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("enable", "device_id");

			var device_id = Parameters["device_id"];
			var enable = Convert.ToBoolean(Parameters["enable"]);

			using (var db = new MyDbContext())
			{
				var q = from dev in db.Object.Devices
						where dev.device_id == device_id
						select dev;

				var device = q.FirstOrDefault();

				if (device == null)
					throw new ArgumentException("No such device_id");

				device.sync_disabled = !enable;

				db.Object.SaveChanges();
			}

			if (!enable)
			{
				var conn = ConnectedClientCollection.Instance.GetAllConnections().Where(x => x.device_id == device_id).FirstOrDefault();
				if (conn != null)
				{
					conn.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "sync disabled");
				}
			}

			respondSuccess();
		}
	}
}
