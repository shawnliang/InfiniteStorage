using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelRenameApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("label_id", "name");

			var label_id = new Guid(Parameters["label_id"]);
			var name = Parameters["name"];

			using (var db = new MyDbContext())
			{
				var label = (from lb in db.Object.Labels
							 where lb.label_id == label_id
							 select lb).FirstOrDefault();

				if (label == null)
					throw new Exception("label_id not exist:" + label_id);


				label.name = name;
				label.seq = SeqNum.GetNextSeq();
				db.Object.SaveChanges();
			}

			respondSuccess();
		}
	}
}
