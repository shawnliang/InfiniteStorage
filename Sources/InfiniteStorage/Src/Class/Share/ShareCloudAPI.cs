using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using postServiceLibrary;
using InfiniteStorage.Cloud;
using InfiniteStorage.Properties;
using InfiniteStorage.Model;
using System.IO;

namespace InfiniteStorage.Share
{
	class ShareCloudAPI : IShareCloudAPI
	{
		public void UploadAttachment(Model.FileAsset file)
		{
			if (file.type == (int)FileAssetType.image)
			{
				var file_data = getAppropriateThumbnail(file);

				if (file_data == null)
				{
					log4net.LogManager.GetLogger(GetType()).WarnFormat("!STRANGE! file does not exist: {0}, {1}", file.file_id, file.saved_path);
					return;
				}

				postServiceClass.attachments_upload(file_data, CloudService.SessionToken, Settings.Default.GroupId, file.file_name, "", "", "image", "medium", file.file_id.ToString(), null, CloudService.APIKey, file.event_time);
			}
			else
			{
				log4net.LogManager.GetLogger(GetType()).Warn("video upload is not implemented yet");
			}
		}

		private byte[] getAppropriateThumbnail(FileAsset file)
		{
			var large = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() +".large.thumb");
			if (File.Exists(large))
				return File.ReadAllBytes(large);

			var medium = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() +".medium.thumb");
			if (File.Exists(medium))
				return File.ReadAllBytes(medium);

			var small = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() + ".small.thumb");
			if (File.Exists(small))
				return File.ReadAllBytes(small);

			var origin = Path.Combine(MyFileFolder.Photo, file.saved_path);
			if (File.Exists(origin))
				return File.ReadAllBytes(origin);

			return null;
		}

		public void CreatePost(Model.Label label, ICollection<Model.ShareRecipient> recipients, ICollection<Model.FileAsset> files)
		{
			throw new NotImplementedException();
		}

		public void UpdatePost(Model.Label label, ICollection<Model.ShareRecipient> recipients, ICollection<Model.FileAsset> files)
		{
			var api = new postServiceClass()
			{
				session_token = CloudService.SessionToken,
				APIKEY = CloudService.APIKey,
				group_id = Settings.Default.GroupId
			};

			api.UpdatePost(api.session_token, label.share_post_id, files.Select(x => x.file_id.ToString()).ToList(), DateTime.Now);
		}
	}
}
