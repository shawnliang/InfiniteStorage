using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Properties;
using System.IO;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	class ConnectMsgHandlerUtil : IConnectMsgHandlerUtil
	{
		public Device GetClientInfo(string device_id)
		{
			using (var db = new MyDbContext())
			{
				var query = from dev in db.Object.Devices
							where dev.device_id == device_id
							select dev;

				return query.FirstOrDefault();
			}
		}

		public string GetServerId()
		{
			return Settings.Default.ServerId;
		}

		public string GetPhotoFolder()
		{
			return MyFileFolder.Photo;
		}

		public long GetFreeSpace(string path)
		{
			var drive = new DriveInfo(Path.GetPathRoot(path));
			return drive.AvailableFreeSpace;
		}

		public void Save(Device clientInfo)
		{
			using (var db = new MyDbContext())
			{
				db.Object.Devices.Add(clientInfo);
				db.Object.SaveChanges();
			}
		}

		public DeviceSummary GetDeviceSummary(string device_id)
		{
			using (var db = new MyDbContext())
			{
				var result = from f in db.Object.Files
							 where f.device_id == device_id
							 group f by device_id
								 into g
								 select new DeviceSummary
								 {
									 photo_count = g.Count(x => x.type == (int)FileAssetType.image),
									 audio_count = g.Count(x => x.type == (int)FileAssetType.audio),
									 video_count = g.Count(x => x.type == (int)FileAssetType.video),
									 backup_range = new TimeRange
									 {
										 start = g.Min(x => x.event_time),
										 end = g.Max(x => x.event_time)
									 }
								 };

				return result.FirstOrDefault();
			}
		}


		public string GetUniqueDeviceFolder(string device_name)
		{
			var sanitizedName = sanitize(ref device_name);
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

		private static List<string> getAllDevFolderNames()
		{
			using (var db = new MyDbContext())
			{
				return db.Object.Devices.Select(x => x.device_name).ToList();
			}

		}

		private static string sanitize(ref string device_name)
		{
			foreach (var illege_char in Path.GetInvalidFileNameChars())
			{
				device_name = device_name.Replace(illege_char, '-');
			}

			var sanitizedName = device_name;
			return sanitizedName;
		}


		public bool RejectUnpairedDevices
		{
			get { return Settings.Default.RejectOtherDevices; }
		}
	}
}
