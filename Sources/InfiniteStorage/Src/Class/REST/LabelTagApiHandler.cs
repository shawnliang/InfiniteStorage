using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

namespace InfiniteStorage.REST
{
	class LabelTagApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("file_id", "label_id");

			var file_ids = Parameters["file_id"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x)).ToList();
			var label_id = new Guid(Parameters["label_id"]);

			using (var db = new MyDbContext())
			{

				foreach (var file_id in file_ids)
				{
					bool alreadyTaged = (from lf in db.Object.LabelFiles
										 where lf.label_id == label_id && lf.file_id == file_id
										 select lf).Any();

					if (!alreadyTaged)
					{
						var label = (from lb in db.Object.Labels
									 where lb.label_id == label_id
									 select lb).FirstOrDefault();

						if (label == null)
							throw new Exception("No such label_id: " + label_id);

						var fileExists = (from f in db.Object.Files
										  where f.file_id == file_id
										  select new { a = 1 }).Any();

						if (!fileExists)
							throw new Exception("No such file_id: " + file_id);


						label.seq = SeqNum.GetNextSeq();
						db.Object.LabelFiles.Add(new LabeledFile { file_id = file_id, label_id = label_id });
					}
				}

				db.Object.SaveChanges();
			}

			respondSuccess();
		}
	}
}
