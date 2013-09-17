#region

using System;
using System.IO;

#endregion

namespace InfiniteStorage
{
	internal static class MediaLibrary
	{
		private static string userFolder;

		public static string UserFolder
		{
			get
			{
				if (userFolder == null)
					userFolder = Environment.GetEnvironmentVariable("UserProfile");
				return userFolder;
			}
		}

		public static string MyPictures
		{
			get { return Path.Combine(userFolder, "Pictures"); }
		}

		public static string MyVideos
		{
			get { return Path.Combine(userFolder, "Videos"); }
		}

		public static string MyPodcasts
		{
			get { return Path.Combine(userFolder, "Podcasts"); }
		}
	}
}