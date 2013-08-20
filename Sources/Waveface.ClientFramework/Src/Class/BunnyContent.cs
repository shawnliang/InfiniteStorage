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
						if (!File.Exists(Uri.LocalPath))
							return null;

						if (Type == ContentType.Video)
							return null;

                        _imageSource = BitmapFrame.Create(Uri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

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

						if (imageFrame != null && imageFrame.Thumbnail != null)
							return imageFrame.Thumbnail;

						return ImageSource;
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
			var imageFile = string.Format(@"{0}\.thumbs\{1}.{2}.thumb", resourceFolderValue, this.ID, this.Type == ContentType.Photo ? "small" : "medium");
			return imageFile;
		}

		private bool GetLiked()
		{
			using (var conn = BunnyDB.CreateConnection())
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT 1 FROM LabelFiles where file_id = @fid and label_id = @label";
					cmd.Parameters.Add(new SQLiteParameter("@fid", new Guid(ID)));
					cmd.Parameters.Add(new SQLiteParameter("@label", Guid.Empty));

					var liked = cmd.ExecuteScalar() != null;
					return liked;
				}
			}
		}
		#endregion



		#region Public Method

		public override void Refresh()
		{
			base.Refresh();
			this.Liked = GetLiked();
		}

		public override bool Equals(object obj)
		{
			//檢查參數是否為null
			if (obj == null)
				return false;

			//檢查是否與自身是相同物件
			if (object.ReferenceEquals(this, obj))
				return true;

			//檢查是否型態相等
			var value = obj as ContentEntity;
			if (value == null)
				return false;

			//比較內容是否相等
			return this.Uri.LocalPath == value.Uri.LocalPath;
		}

		public override int GetHashCode()
		{
			return this.Uri.LocalPath.GetHashCode();
		}
		#endregion
	}
}
