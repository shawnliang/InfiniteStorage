using InfiniteStorage.Properties;
using System;
using System.IO;

namespace InfiniteStorage
{
	static class MyFileFolder
	{
		public static string Temp
		{
			get
			{
				var settings = Settings.Default;

				switch ((LocationType)settings.LocationType)
				{
					case LocationType.SingleFolder:
						return Path.Combine(settings.SingleFolderLocation, ".temp");

					case LocationType.MediaLibrary:
						return Path.Combine(MediaLibrary.UserFolder, ".temp");

					case LocationType.Custom:
						return Path.Combine(settings.CustomPhotoLocation, ".temp");

					default:
						throw new NotImplementedException();
				}
			}
		}

		public static string Pending
		{
			get
			{
				return Path.Combine(Settings.Default.SingleFolderLocation, ".pending");
			}
		}

		public static string Thumbs
		{
			get
			{
				return Path.Combine(Settings.Default.SingleFolderLocation, ".thumbs");
			}
		}

		public static string Photo
		{
			get
			{
				var settings = Settings.Default;

				switch ((LocationType)settings.LocationType)
				{
					case LocationType.SingleFolder:
						return settings.SingleFolderLocation;


					case LocationType.MediaLibrary:
						return Path.Combine(MediaLibrary.MyPictures, Resources.ProductName);

					case LocationType.Custom:
						return settings.CustomPhotoLocation;

					default:
						throw new NotImplementedException();
				}
			}
		}

		public static string Video
		{
			get
			{
				var settings = Settings.Default;

				switch ((LocationType)settings.LocationType)
				{
					case LocationType.SingleFolder:
						return settings.SingleFolderLocation;


					case LocationType.MediaLibrary:
						return Path.Combine(MediaLibrary.MyVideos, Resources.ProductName);

					case LocationType.Custom:
						return settings.CustomVideoLocation;

					default:
						throw new NotImplementedException();
				}
			}
		}


		public static string Audio
		{
			get
			{
				var settings = Settings.Default;

				switch ((LocationType)settings.LocationType)
				{
					case LocationType.SingleFolder:
						return settings.SingleFolderLocation;


					case LocationType.MediaLibrary:
						return Path.Combine(MediaLibrary.MyPodcasts, Resources.ProductName);

					case LocationType.Custom:
						return settings.CustomAudioLocation;

					default:
						throw new NotImplementedException();
				}
			}
		}

		public static string AppData
		{
			get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bunny"); }
		}
	}
}
