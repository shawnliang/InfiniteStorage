using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;

namespace InfiniteStorage.REST
{
	class LabelShareApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("enabled", "label_id");

			var label_id = new Guid(Parameters["label_id"]);
			var enabled = Convert.ToBoolean(Parameters["enabled"]);

			using (var db = new MyDbContext())
			{
				var q = from lb in db.Object.Labels
						where lb.label_id == label_id
						select lb;

				var label = q.FirstOrDefault();

				if (label == null)
					throw new Exception("no such label_id:" + label_id.ToString());

				label.share_enabled = enabled;
				label.seq = SeqNum.GetNextSeq();

				if (enabled)
				{
					if (!string.IsNullOrEmpty(label.share_post_id))
				}

				db.Object.SaveChanges();
			}
		}
	}
}
