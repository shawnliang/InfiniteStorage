using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using postServiceLibrary;

namespace InfiniteStorage.REST
{
	class LabelInviteApiHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter("label_id", "recipients");

			var label_id = new Guid(Parameters["label_id"]);
			var recipients = getRecipients();
			var sender = Parameters["sender"];
			var msg = Parameters["msg"];

			if (recipients == null || !recipients.Any())
				throw new Exception("no recipients");

			var label = getLabel(label_id);

			callCloudApiToSendEmail(label, sender, msg, recipients);
			updateSender(label, sender);

			respondSuccess();
		}

		private void updateSender(Label label, string sender)
		{
			var ws = new postServiceClass
			{
				session_token = Cloud.CloudService.SessionToken,
				APIKEY = Cloud.CloudService.APIKey,
				group_id = Settings.Default.GroupId
			};

			var attList = getAttachmentList(label);

			ws.UpdatePost(ws.session_token, label.share_post_id, attList, DateTime.Now.AddDays(-1.0), new List<string>(), label.name, sender);
		}

		private List<string> getAttachmentList(Label label)
		{
			using (var db = new MyDbContext())
			{
				var q = from lf in db.Object.LabelFiles
						join f in db.Object.Files on lf.file_id equals f.file_id
						where lf.label_id == label.label_id
						orderby f.event_time ascending
						select f.file_id;

				return q.ToList().Select(x => x.ToString()).ToList();
			}
		}

		private void callCloudApiToSendEmail(Label label, string sender, string msg, ICollection<Recipient> recipients)
		{
			if (string.IsNullOrEmpty(label.share_code))
				throw new Exception("label is not shared yet: " + label.label_id + " " + label.name);

			postServiceLibrary.postServiceClass.sendFavoriteEmail(
				Cloud.CloudService.SessionToken,
				Cloud.CloudService.APIKey,
				recipients.Select(x=>x.email).ToList(),
				label.share_code,
				sender,
				label.name,
				msg);
		}

		private static Model.Label getLabel(Guid label_id)
		{
			using (var db = new MyDbContext())
			{
				var q = from lb in db.Object.Labels
						where lb.label_id == label_id
						select lb;

				var label = q.FirstOrDefault();

				if (label == null || label.deleted == true)
					throw new Exception("label does not exist or is already deleted: " + label_id);

				return label;
			}
		}

		private List<Recipient> getRecipients()
		{
			List<Recipient> recipients;
			try
			{
				recipients = JsonConvert.DeserializeObject<List<Recipient>>(Parameters["recipients"]);
			}
			catch (Exception err)
			{
				throw new FormatException("recipient format is invalid: " + Parameters["recipients"], err);
			}
			return recipients;
		}
	}

	class Recipient
	{
		public string email { get; set; }
		public string name { get; set; }
	}
}
