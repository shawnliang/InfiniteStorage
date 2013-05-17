using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using InfiniteStorage.Model;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace InfiniteStorage.REST
{
	class LabelGetApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{

			CheckParameter("label_id");

			var label_id = new Guid(Parameters["label_id"]);

			object result;


			using (var db = new MyDbContext())
			{
				var label = (from lb in db.Object.Labels
						 where lb.label_id == label_id
						 select lb).FirstOrDefault();


				if (label == null)
					throw new Exception("label not exist: " + label_id.ToString());

				var files = new Notify.NotifySenderUtil().QueryLabeledFiles(label_id).Select(x => x.id);

				result = new
				{
					label_id = label_id,
					label_name = label.name,
					seq = label.seq,
					files = files.ToList()
				};
			}

			respondSuccess(result);
		}
	}
}
