using InfiniteStorage.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Wammer.Utility;

namespace InfiniteStorage
{
	class ThumbnailCreator
	{
		private System.Timers.Timer timer = new System.Timers.Timer(1000.0);
		private long from = 0;
		private string thumbnailLocation;
		private bool started;
		private object cs = new object();
		private static ThumbnailCreator instance = new ThumbnailCreator();

		public static ThumbnailCreator Instance
		{
			get { return instance; }
		}

		private ThumbnailCreator()
		{
			timer.Elapsed += timer_Elapsed;
			thumbnailLocation = Path.Combine(MyFileFolder.Photo, ".thumbs");

			if (!Directory.Exists(thumbnailLocation))
			{
				Directory.CreateDirectory(thumbnailLocation);

				var dirinfo = new DirectoryInfo(thumbnailLocation);
				dirinfo.Attributes |= FileAttributes.Hidden;
			}
		}

		public void Start()
		{
			timer.Start();
			started = true;
		}

		public void Stop()
		{
			timer.Stop();
			started = false;
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (cs)
			{
				timer.Stop();

				try
				{
					generateThumbnails();
				}
				catch (Exception err)
				{
					log4net.LogManager.GetLogger(GetType()).Warn("Failed to generate thumbnails", err);
				}
				finally
				{
					if (started)
						timer.Start();
				}
			}
		}

		private void generateThumbnails()
		{
			List<PendingFile> files = getNoThumbnailFiles(from);

			var successFiles = new List<PendingFile>();
			foreach (var file in files)
			{
				try
				{
					int width, height;
					generateThumbnail(file, out width, out height);


					file.width = width;
					file.height = height;
					successFiles.Add(file);
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

		private void markFilesHavingThumbnail(List<PendingFile> successFiles)
		{
			using (var conn = new SQLiteConnection(MyDbContext.ConnectionString))
			{
				conn.Open();

				using (var trans = conn.BeginTransaction())
				{
					var cmd = conn.CreateCommand();
					cmd.Connection = conn;
					cmd.CommandText = "update [PendingFiles] set thumb_ready = 1, width = @width, height = @height where file_id = @fid";
					cmd.CommandType = System.Data.CommandType.Text;

					var fid = new SQLiteParameter("@fid");
					var width = new SQLiteParameter("@width");
					var height = new SQLiteParameter("@height");

					cmd.Parameters.Add(fid);
					cmd.Parameters.Add(width);
					cmd.Parameters.Add(height);

					foreach (var file in successFiles)
					{
						fid.Value = file.file_id;
						width.Value = file.width;
						height.Value = file.height;

						cmd.ExecuteNonQuery();
					}

					trans.Commit();
				}
			}
		}

		private void generateThumbnail(PendingFile file, out int width, out int height)
		{
			var file_path = Path.Combine(MyFileFolder.Photo, ".pending", file.saved_path);

			using (var m = readFilesToMemory(file_path))
			using (var fullImage = new Bitmap(m))
			{
				width = fullImage.Width;
				height = fullImage.Height;

				var longSide = Math.Max(fullImage.Width, fullImage.Height);

				if (longSide < 256)
					return;

				var orientation = ImageHelper.ImageOrientation(fullImage);

				if (longSide > 2048)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 2048))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".large.thumb"), ImageFormat.Jpeg);
					}
				}

				if (longSide > 1024)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 1024))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".medium.thumb"), ImageFormat.Jpeg);
					}
				}

				if (longSide > 512)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 512))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".small.thumb"), ImageFormat.Jpeg);
					}
				}

				if (longSide > 256)
				{
					using (var thum = ImageHelper.ScaleBasedOnLongSide(fullImage, 256))
					{
						ImageHelper.CorrectOrientation(orientation, thum);
						thum.Save(Path.Combine(thumbnailLocation, file.file_id.ToString() + ".tiny.thumb"), ImageFormat.Jpeg);
					}
				}
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

		private List<PendingFile> getNoThumbnailFiles(long fromSeq)
		{
			using (var db = new MyDbContext())
			{
				var query = from f in db.Object.PendingFiles
							where f.seq >= fromSeq && !f.deleted && !f.thumb_ready && f.type == (int)FileAssetType.image
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
