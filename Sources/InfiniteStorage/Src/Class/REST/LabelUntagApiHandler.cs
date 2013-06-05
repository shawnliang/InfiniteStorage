using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelUntagApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("file_id", "label_id");

			var file_ids = Parameters["file_id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x)).ToList();
			var label_id = new Guid(Parameters["label_id"]);

			using (var db = new MyDbContext())
			{
				var label = (from lb in db.Object.Labels
							 where lb.label_id == label_id
							 select lb).FirstOrDefault();

				if (label == null)
					throw new ArgumentException("no such label_id: " + label_id.ToString());

				foreach (var file_id in file_ids)
				{
					var labeledFile = (from lb in db.Object.LabelFiles
									   where lb.label_id == label_id && lb.file_id == file_id
									   select lb).FirstOrDefault();

					if (labeledFile != null)
					{
						db.Object.LabelFiles.Remove(labeledFile);
						label.seq = SeqNum.GetNextSeq();
					}
				}

				db.Object.SaveChanges();
			}

			respondSuccess();
		}
	}
}
