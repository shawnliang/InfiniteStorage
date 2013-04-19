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
							 group f by device_id
								 into g
								 select new DeviceSummary
								 {
									 photo_count = g.Count(x=>x.type== FileAssetType.image),
									 audio_count = g.Count(x=>x.type == FileAssetType.audio),
									 video_count = g.Count(x => x.type == FileAssetType.video),
									 backup_range = new TimeRange( g.Min(x=>x.event_time), g.Max(x=>x.event_time))
								 };

				return result.FirstOrDefault();
			}
		}
	}
}
