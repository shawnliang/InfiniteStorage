using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;
using System.Net;

namespace InfiniteStorage.REST
{
	class LabelUntagApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("file_id", "label_id");

			var file_id = new Guid(Parameters["file_id"]);
			var label_id = new Guid(Parameters["label_id"]);

			using (var db = new MyDbContext())
			{
				var labeledFile = (from lb in db.Object.LabelFiles
								  where lb.label_id == label_id && lb.file_id == file_id
								  select lb).FirstOrDefault();

				if (labeledFile != null)
				{
					db.Object.LabelFiles.Remove(labeledFile);

					var label = (from lb in db.Object.Labels
								 where lb.label_id == label_id
								 select lb).FirstOrDefault();

					if (label != null)
						label.seq = SeqNum.GetNextSeq();

					db.Object.SaveChanges();
				}

			}

			respondSuccess();
		}
	}
}
