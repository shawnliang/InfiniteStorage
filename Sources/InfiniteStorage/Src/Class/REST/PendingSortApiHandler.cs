using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using Newtonsoft.Json;

namespace InfiniteStorage.REST
{
	class PendingSortApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			CheckParameter("how");

			var how = JsonConvert.DeserializeObject<PendingSortData>(Parameters["how"]);

			respondSuccess();
		}
	}
}
