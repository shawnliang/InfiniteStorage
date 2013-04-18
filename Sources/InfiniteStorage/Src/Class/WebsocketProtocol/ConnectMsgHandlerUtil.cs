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


		public TimeRange GetBackupRange(string device_id)
		{
			using (var db = new MyDbContext())
			{
				var result = from f in db.Object.Files
							 group f by device_id
								 into g
								 select new
								 {
									 min = g.Min(x=>x.event_time),
									 max = g.Max(x=>x.event_time)
								 };

				var minMax = result.FirstOrDefault();

				if (minMax != null)
					return new TimeRange(minMax.min, minMax.max);
				else
					return null;
			}
		}
	}
}
