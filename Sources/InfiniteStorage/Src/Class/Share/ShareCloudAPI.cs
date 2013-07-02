using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using postServiceLibrary;
using InfiniteStorage.Cloud;
using InfiniteStorage.Properties;
using InfiniteStorage.Model;
using System.IO;

using Microsoft.WindowsAPICodePack.Shell;
using InfiniteStorage.Common;

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

				var file_data = getVideoThumbnail(file);

				if (file_data == null)
				{
					log4net.LogManager.GetLogger(GetType()).WarnFormat("{1} seems not a valid video... {0}", file.file_id, file.saved_path);
					return;
				}

				postServiceClass.attachments_upload(file_data, CloudService.SessionToken, Settings.Default.GroupId, file.file_name + ".mp4", "", "", "video", "medium", file.file_id.ToString(), null, CloudService.APIKey, file.event_time);
			}
		}

		private byte[] getVideoThumbnail(FileAsset file)
		{
			var thumb_path = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() + ".thumb.mp4");

			if (File.Exists(thumb_path))
				File.Delete(thumb_path);

			var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);

			var videoWidth = getVideoWidth(file_path);


			if (videoWidth <= 0)
				return null;

			if (videoWidth > 720)
				FFmpegHelper.MakeVideoThumbnail720(file_path, thumb_path);
			else
				FFmpegHelper.MakeVideoThumbnail(file_path, thumb_path);

			return File.ReadAllBytes(thumb_path);
		}

		private static int getVideoWidth(string file_path)
		{
			try
			{
				var fileInfo = ShellFile.FromFilePath(file_path);
				var videoWidth = (int)fileInfo.Properties.System.Video.FrameWidth.Value;
				return videoWidth;
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(typeof(ShareCloudAPI)).WarnFormat("Unable to get video width: " + file_path, err);
				return -1;
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

			if (recipients == null)
				recipients = new List<Model.ShareRecipient>();

			if (files == null)
				files = new List<Model.FileAsset>();

			// use an old last_update_time to work around cloud consistency checking
			var lastUpdateTime = DateTime.Now.AddMinutes(-10.0);
			api.UpdatePost(
				api.session_token,
				label.share_post_id,
				files.Select(x => x.file_id.ToString()).ToList(),
				lastUpdateTime,
				recipients.Select(x=>x.email).ToList(),
				label.name,
				""
				);
		}
	}
}
