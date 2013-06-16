using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.Share
{
	public class ShareEnableTask : IShareTask
	{
		private IShareEnableTaskDB db;
		private IShareCloudAPI api;

		public ShareEnableTask()
		{
			this.db = new ShareEnableTaskDB();

			throw new NotImplementedException();
		}

		public ShareEnableTask(IShareEnableTaskDB db, IShareCloudAPI api)
		{
			this.db = db;
			this.api = api;
		}

		public void Process(Model.Label label)
		{
			var files = db.QueryLabelFiles(label);
			foreach (var file in files.Where(x=>!x.on_cloud.HasValue || !x.on_cloud.Value))
			{
				api.UploadAttachment(file);
				db.UpdateFileOnCloud(file);
			}

			var recipients = db.QueryRecipients(label);

			if (string.IsNullOrEmpty(label.share_post_id))
				api.CreatePost(label, recipients, files);
			else
				api.UpdatePost(label, recipients, files);

			foreach (var recipient in recipients.Where(x => !x.on_cloud.HasValue || !x.on_cloud.Value))
			{
				db.UpdateRecipientOnCloud(recipient);
			}



			db.UpdateShareComplete(label);
		}
	}
}
