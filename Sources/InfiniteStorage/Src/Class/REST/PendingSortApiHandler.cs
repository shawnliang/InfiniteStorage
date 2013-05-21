using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class PendingSortApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			CheckParameter("how");


			respondSuccess();
		}
	}
}
