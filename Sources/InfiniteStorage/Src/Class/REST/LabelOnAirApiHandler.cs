using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelOnAirApiHandler : HttpHandler
	{

		public override void HandleRequest()
		{
			CheckParameter("label_id", "on_air");

			var label_id = new Guid(Parameters["label_id"]);
			var on_air = (Parameters["on_air"] == "true");

			using (var db = new MyDbContext())
			{
				var q = (from f in db.Object.Labels
						 where f.label_id == label_id
						 select f).FirstOrDefault();

				if (q == null)
					throw new Exception("No such label: " + label_id.ToString());

				q.on_air = on_air;
				q.seq = SeqNum.GetNextSeq();

				db.Object.SaveChanges();
			}

			respondSuccess();
		}
	}
}
