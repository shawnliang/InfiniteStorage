using InfiniteStorage.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Wammer.Utility;
using Waveface.Common;
using System.Diagnostics;
using System.Reflection;

namespace InfiniteStorage
{
	class ThumbnailCreator
	{
		private NoReentrantTimer timer;
		private long from = 0;
		private string thumbnailLocation;
		private object cs = new object();


		public ThumbnailCreator()
		{
			thumbnailLocation = MyFileFolder.Thumbs;

			if (!Directory.Exists(thumbnailLocation))
			{
				Directory.CreateDirectory(thumbnailLocation);

				var dirinfo = new DirectoryInfo(thumbnailLocation);
				dirinfo.Attributes |= FileAttributes.Hidden;
			}


			timer = new NoReentrantTimer(timer_Elapsed, null, 5000, 1000);
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}

		void timer_Elapsed(object nil)
		{
			try
			{
				generateThumbnails();
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Failed to generate thumbnails", err);
			}
		}

		private void generateThumbnails()
		{
			var files = getNoThumbnailFiles(from);

			var successFiles = new List<FileAsset>();
			foreach (var file in files)
			{
				try
				{
					int width = 0, height = 0;
					if (file.type == (int)FileAssetType.image)
						generateThumbnail(file, out width, out height);
					else if (file.type == (int)FileAssetType.video)
						extractStillImage(file);
					else
						continue;


					file.width = width;
					file.height = height;
					successFiles.Add(file);
				}
				catch (BadBitmapException e)
				{
					log4net.LogManager.GetLogger(GetType()).Warn(file.file_path + "is a bad image. delete it", e);
					deleteFile(file);
				}
				catch (Exception e)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Unable to generate thumbnail: " + file.saved_path, e);
				}
			}

			markFilesHavingThumbnail(successFiles);

			if (successFiles.Any())
				from = successFiles.Max(x => x.seq);
		}

		private void deleteFile(FileAsset file)
		{
			try{
				var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);
				File.Delete(file_path);
			}
			catch(Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to delete a file", err);
			}

			try
			{
				using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
				{
					conn.Open();

					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = "update files set deleted = 1 where file_id = @file";
						cmd.Prepare();
						cmd.Parameters.Add(new SQLiteParameter("@file", file.file_id));
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception err)
			{
				log4net.LogManager.GetLogger(GetType()).Warn("Unable to mark file record deleted", err);
			}
		}

		private void extractStillImage(FileAsset file)
		{
			var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);

			var thumb_path = Path.Combine(MyFileFolder.Thumbs, file.file_id.ToString() + ".medium.thumb");

			if (File.Exists(thumb_path))
				File.Delete(thumb_path);

			using (var process = new Process())
			{
				process.StartInfo = new ProcessStartInfo
					{
						FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg.exe"),
						Arguments = string.Format("-i \"{0}\" -r 1 -t 1 -s vga -f image2 \"{1}\"", file_path, thumb_path),
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden
					};

				process.Start();
				if (!process.WaitForExit(30 * 1000))
					throw new Exception("ffmpeg does not exit in 30 secs: " + process.StartInfo.Arguments);

				if (!File.Exists(thumb_path))
					throw new Exception("ffmpeg failed extract still image:" + process.StartInfo.Arguments);
			}
		}

		private void markFilesHavingThumbnail(List<FileAsset> successFiles)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var trans = conn.BeginTransaction())
				using (var cmd = conn.CreateCommand())
				{

					cmd.Connection = conn;
					cmd.CommandText = "update [Files] set thumb_ready = 1, width = @width, height = @height, orientation = @ori where file_id = @fid";
					cmd.CommandType = System.Data.CommandType.Text;

					var fid = new SQLiteParameter("@fid");
					var width = new SQLiteParameter("@width");
					var height = new SQLiteParameter("@height");
					var ori = new SQLiteParameter("@ori");

					cmd.Parameters.Add(fid);
					cmd.Parameters.Add(width);
					cmd.Parameters.Add(height);
					cmd.Parameters.Add(ori);

					cmd.Prepare();

					foreach (var file in successFiles)
					{
						fid.Value = file.file_id;
						width.Value = file.width;
						height.Value = file.height;
						ori.Value = file.orientation;
						cmd.ExecuteNonQuery();
					}

					trans.Commit();
				}
			}
		}

		private void generateThumbnail(FileAsset file, out int width, out int height)
		{
			var file_path = Path.Combine(MyFileFolder.Photo, file.saved_path);

			using (var m = readFilesToMemory(file_path))
			using (var fullImage = createBitmap(m))
			{
				width = fullImage.Width;
				height = fullImage.Height;

				var longSide = Math.Max(fullImage.Width, fullImage.Height);

				if (longSide < 256)
					return;

				var orientation = ImageHelper.ImageOrientation(fullImage);
				file.orientation = (int)orientation;

				if (longSide > 2048)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 2048))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".large.thumb"), ImageFormat.Jpeg);
					}
				}
				else
				{
					fullImage.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".large.thumb"), ImageFormat.Jpeg);
				}

				if (longSide > 1024)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 1024))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".medium.thumb"), ImageFormat.Jpeg);
					}
				}
				else
				{
					fullImage.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".medium.thumb"), ImageFormat.Jpeg);
				}

				if (longSide > 512)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 512))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".small.thumb"), ImageFormat.Jpeg);
					}
				}
				else
				{
					fullImage.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".small.thumb"), ImageFormat.Jpeg);
				}

				if (longSide > 256)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 256))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".tiny.thumb"), ImageFormat.Jpeg);
					}
				}
				else
				{
					fullImage.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".tiny.thumb"), ImageFormat.Jpeg);
				}
			}
		}

		private Bitmap createBitmap(Stream m)
		{
			try
			{
				return new Bitmap(m);
			}
			catch (Exception err)
			{
				throw new BadBitmapException("Bad image", err);
			}
		}

		private static MemoryStream readFilesToMemory(string file)
		{
			var m = new MemoryStream();

			using (var f = File.OpenRead(file))
			{
				f.CopyTo(m);
			}

			m.Position = 0;
			return m;
		}

		private List<FileAsset> getNoThumbnailFiles(long fromSeq)
		{
			using (var db = new MyDbContext())
			{
				var query = from f in db.Object.Files
							where f.seq >= fromSeq && !f.deleted && !f.thumb_ready
							orderby f.seq ascending
							select f;

				return query.ToList();
			}
		}

		private Dictionary<string, Device> getAllDevices()
		{
			using (var db = new MyDbContext())
			{
				return (from d in db.Object.Devices
						select d).ToDictionary(x => x.device_id);
			}
		}
	}
}
