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

				
				label.seq = SeqNum.GetNextSeq();

				if (enabled)
				{
					if (string.IsNullOrEmpty(label.share_post_id))
					{
						var post_id = Guid.NewGuid().ToString();
						var api = Cloud.CloudService.CreateCloudAPI();
						var shared_code = api.NewPost(api.session_token, new List<string>(), new List<string>(), post_id);

						label.share_post_id = post_id;
						label.share_code = shared_code;
						label.share_enabled = enabled;
					}
					else if (!string.IsNullOrEmpty(label.share_code) && (!label.share_enabled.HasValue || !label.share_enabled.Value))
					{
						var api = Cloud.CloudService.CreateCloudAPI();
						api.tuneOnSharedcode(api.session_token, label.share_post_id);
					}
				}
				else
				{
					var api = Cloud.CloudService.CreateCloudAPI();
					api.tuneOffSharedcode(api.session_token, label.share_post_id);
					label.share_enabled = enabled;
				}

				db.Object.SaveChanges();


				if (enabled)
					respondSuccess(new { shared_code = label.share_code, status = 200, api_ret_code = 0, api_ret_message = "success" });
				else
					respondSuccess();
			}
		}
	}
}
