﻿using InfiniteStorage.Model;
using System;
using System.Linq;
using Wammer.Station;

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
					auto_type = label.auto_type,
					on_air = label.on_air,
					files = files.ToList()
				};
			}

			respondSuccess(result);
		}
	}
}
