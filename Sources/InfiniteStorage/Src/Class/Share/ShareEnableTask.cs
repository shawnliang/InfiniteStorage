using System.Linq;

namespace InfiniteStorage.Share
{
	public class ShareEnableTask : IShareTask
	{
		private IShareEnableTaskDB db;
		private IShareCloudAPI api;

		public ShareEnableTask()
		{
			this.db = new ShareEnableTaskDB();
			this.api = new ShareCloudAPI();
		}

		public ShareEnableTask(IShareEnableTaskDB db, IShareCloudAPI api)
		{
			this.db = db;
			this.api = api;
		}

		public void Process(Model.Label label)
		{
			var files = db.QueryLabelFiles(label);

			var OnCloudFiles = files.Where(x => x.on_cloud.HasValue && x.on_cloud.Value).ToList();

			foreach (var file in files.Where(x => !x.on_cloud.HasValue || !x.on_cloud.Value))
			{
				api.UploadAttachment(file);
				db.UpdateFileOnCloud(file);

				OnCloudFiles.Add(file);
				OnCloudFiles.Sort((x, y) => x.event_time.CompareTo(y.event_time));

				api.UpdatePost(label, null, OnCloudFiles);
			}


			if (string.IsNullOrEmpty(label.share_post_id))
				api.CreatePost(label, null, files);
			else
				api.UpdatePost(label, null, files);


			db.UpdateShareComplete(label);
		}
	}
}
