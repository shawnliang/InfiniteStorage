using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using Newtonsoft.Json;
using InfiniteStorage.Model;

namespace InfiniteStorage.REST
{
	class PendingSortApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			CheckParameter("how");

			var how = JsonConvert.DeserializeObject<PendingSortData>(Parameters["how"]);

			var pendingToResource = new PendingToResource(
				new PendingToResourceUtil(MyDbContext.ConnectionString, how.device_id, MyFileFolder.Photo));

			foreach (var evt in how.events)
			{
				pendingToResource.Do(evt);
			}

			respondSuccess();
		}
	}
}
