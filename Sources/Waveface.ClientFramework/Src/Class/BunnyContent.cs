using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Waveface.Model;

namespace Waveface.ClientFramework
{
	public class BunnyContent : Content
	{
		#region Var
		private bool _likedInited;
		private BitmapSource _imageSource;
		private BitmapSource _thumbnailSource;
		#endregion


		#region Property
		public Boolean EnableTag { get; set; }

		public BitmapSource ImageSource
		{
			get
			{
				if (_imageSource == null)
				{
					lock (this)
					{
						var file = this.Uri.LocalPath;
						if (!File.Exists(file))
							return null;

						if (this.Type == ContentType.Video)
							return null;

						_imageSource = BitmapFrame.Create(this.Uri);

						var metadata = _imageSource.Metadata as BitmapMetadata;
						if (metadata != null)
						{
							Rotation rotate = Rotation.Rotate0;

							var metaValue = metadata.GetQuery("/app1/{ushort=0}/{ushort=274}");

							if (metaValue != null)
							{
								ushort value = (ushort)metaValue;
								if (value == 6)
								{
									rotate = Rotation.Rotate90;
								}
								else if (value == 8)
								{
									rotate = Rotation.Rotate270;
								}
								else if (value == 3)
								{
									rotate = Rotation.Rotate180;
								}

								var transform = default(RotateTransform);
								switch (rotate)
								{
									case Rotation.Rotate90:
										transform = new RotateTransform(90);
										break;
									case Rotation.Rotate180:
										transform = new RotateTransform(180);
										break;
									case Rotation.Rotate270:
										transform = new RotateTransform(270);
										break;
								}

								if (transform != null)
								{
									_imageSource = new TransformedBitmap(_imageSource, transform);
									_imageSource.Freeze();
								}
							}
						}
					}
				}
				return _imageSource;
			}
		}

		public BitmapSource ThumbnailSource
		{
			get
			{
				if (_thumbnailSource == null)
				{
					var file = GetThumbnailFile();
					if (!File.Exists(file))
					{
						var imageFrame = (this.ImageSource as BitmapFrame);
						if(imageFrame == null)
							return null;

						return imageFrame.Thumbnail ?? imageFrame;
					}
					_thumbnailSource = BitmapFrame.Create(new Uri(file));
				}
				return _thumbnailSource;
			}
		}

		public override System.Drawing.Image Thumbnail
		{
			get
			{
				return GetContentThumbnail();
			}
		}

		public override bool Liked
		{
			get
			{
				if (!_likedInited)
				{
					_likedInited = true;
					base._liked = GetLiked();
				}
				return base.Liked;
			}
			set
			{
				base.Liked = value;
			}
		}
		#endregion


		#region Constructor
		public BunnyContent()
		{

		}

		public BunnyContent(Uri uri, string file_id, ContentType type)
			: base(file_id, uri)
		{
			_type = type;
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Gets the content thumbnail.
		/// </summary>
		/// <returns></returns>
		private System.Drawing.Image GetContentThumbnail()
		{
			var imageFile = GetThumbnailFile();

			return System.Drawing.Image.FromFile(imageFile);
		}

		private string GetThumbnailFile()
		{
			var resourceFolderValue = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("BunnyHome").GetValue("ResourceFolder").ToString();
			var imageFile = string.Format(@"{0}\.thumbs\{1}.small.thumb", resourceFolderValue, this.ID);
			return imageFile;
		}

		private bool GetLiked()
		{
			var savedPath = this.ContentPath;

			var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny");

			var dbFilePath = Path.Combine(appDir, "database.s3db");

			var conn = new SQLiteConnection(string.Format("Data source={0}", dbFilePath));

			conn.Open();

			var cmd = new SQLiteCommand("SELECT 1 FROM LabelFiles f, Labels lb where file_id = @fid and f.label_id = lb.label_id and lb.name = 'STARRED'", conn);
			cmd.Parameters.Add(new SQLiteParameter("@fid", new Guid(ID)));

			var liked = cmd.ExecuteScalar() != null;

			conn.Close();

			return liked;
		}
		#endregion
	}
}
