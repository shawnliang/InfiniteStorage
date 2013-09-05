using InfiniteStorage.Cloud;
using InfiniteStorage.Common;
using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using Microsoft.WindowsAPICodePack.Shell;
using postServiceLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using Wammer.Utility;
using System.Drawing;

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

				postServiceClass.attachments_upload(file_data, CloudService.SessionToken, Settings.Default.GroupId, file.file_name + ".mp4", "", "", "video", "origin", file.file_id.ToString(), null, CloudService.APIKey, file.event_time);


				uploadStillImagePreviews(file);
			}
		}

		private static void uploadStillImagePreviews(Model.FileAsset file)
		{
			var video_path = Path.Combine(MyFileFolder.Photo, file.saved_path);
			var folder = Path.Combine(MyFileFolder.Temp, file.file_id + ".video_previews");

			if (Directory.Exists(folder))
				Directory.Delete(folder, true);

			Directory.CreateDirectory(folder);

			FFmpegHelper.ExtractStillIamge(video_path, 5, 2, folder, 256);

			var preview_files = Directory.GetFiles(folder).ToList();
			preview_files = cropTo128Sqaure(preview_files);

			var previewData = new PreviewData
			{
				fps = 2,
				seq = new List<PreviewFrame>()
			};

			foreach (var prv_file in preview_files)
			{
				var b64 = Convert.ToBase64String(File.ReadAllBytes(prv_file));

				var frame = new PreviewFrame
				{
					duration = 0.5f,
					url = "data:image/jpeg;base64," + b64
				};

				previewData.seq.Add(frame);
			}

			var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(previewData));
			postServiceClass.attachments_upload(payload, CloudService.SessionToken, Settings.Default.GroupId, file.file_name + ".mp4", "", "", "video", "square", file.file_id.ToString(), null, CloudService.APIKey, file.event_time);

		}

		private static List<string> cropTo128Sqaure(List<string> preview_files)
		{
			foreach (var prev in preview_files)
			{
				using (var img = new Bitmap(prev))
				{
					var tmpImage = ImageHelper.ScaleBasedOnShortSide(img, 128);
					int shortSize = ImageHelper.ShortSizeLength(tmpImage);

					int x = (tmpImage.Width - shortSize) / 2;
					int y = (tmpImage.Height - shortSize) / 2;

					tmpImage = ImageHelper.Crop(tmpImage, x, y, shortSize);
					tmpImage.Save(prev + ".square.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
				}
			}

			preview_files = preview_files.Select(x => x + ".square.jpg").ToList();
			preview_files.Sort();
			return preview_files;
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
			var large = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() + ".large.thumb");
			if (File.Exists(large))
				return File.ReadAllBytes(large);

			var medium = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() + ".medium.thumb");
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
				recipients.Select(x => x.email).ToList(),
				label.name,
				""
				);
		}
	}
}
