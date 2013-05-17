using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelAddApiHandler : HttpHandler
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

				if (label != null)
					throw new Exception("label_id already exist:" + label_id);

				label = new Label
				{
					label_id = label_id,
					name = name,
					seq = SeqNum.GetNextSeq()
				};

				db.Object.Labels.Add(label);
				db.Object.SaveChanges();
			}

			respondSuccess();
		}
	}
}
