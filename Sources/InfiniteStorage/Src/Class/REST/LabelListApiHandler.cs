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
	class LabelListApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{

			var util = new Notify.NotifySenderUtil();

			List<Label> labels;

			using (var db = new MyDbContext())
			{
				labels = (from lb in db.Object.Labels
						  where !lb.deleted
						  select lb).ToList();
			}

			var result = new List<object>();

			foreach(var label in labels)
			{
				var data = new
				{
					label_id = label.label_id,
					label_name = label.name,
					files = util.QueryLabeledFiles(label.label_id).Select(x=>x.id).ToList()
				};

				result.Add(data);
			}


			respondSuccess(new { labels = result });
		}
	}
}
