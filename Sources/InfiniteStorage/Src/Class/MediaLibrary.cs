using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InfiniteStorage
{
	static class MediaLibrary
	{
		static string userFolder;

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
			get
			{
				return Path.Combine(userFolder, "Pictures");
			}
		}

		public static string MyVideos
		{
			get
			{
				return Path.Combine(userFolder, "Videos");
			}
		}

		public static string MyPodcasts
		{
			get
			{
				return Path.Combine(userFolder, "Podcasts");
			}
		}
	}
}
