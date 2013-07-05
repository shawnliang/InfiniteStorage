using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			return DeviceUtility.GetUniqueDeviceFolder(device_name);
			
		}

		public bool RejectUnpairedDevices
		{
			get { return Settings.Default.RejectOtherDevices; }
		}
	}
}
