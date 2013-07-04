using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	static class DeviceUtility
	{
		public static string GetUniqueDeviceFolder(string device_name)
		{
			var sanitizedName = sanitize(device_name);
			var allNames = getAllDevFolderNames();
			var newName = sanitizedName;
			var n = 1;

			while (allNames.Contains(newName))
			{
				newName = sanitizedName + string.Format("({0})", n);
				n++;
			}

			return newName;
		}

		private static string sanitize(string device_name)
		{
			foreach (var illege_char in Path.GetInvalidFileNameChars())
			{
				device_name = device_name.Replace(illege_char, '-');
			}

			return device_name;
		}

		private static List<string> getAllDevFolderNames()
		{
			using (var db = new MyDbContext())
			{
				return db.Object.Devices.Select(x => x.device_name).ToList();
			}
		}
	}
}
