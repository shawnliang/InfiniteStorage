using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using InfiniteStorage.Properties;
using System.IO;

namespace InfiniteStorage
{
	class ConnectionStatusUpdator
	{
		public void UpdateStatusToPeers()
		{
			var peers = ConnectedClientCollection.Instance.GetAllConnections();
			var util = new WebsocketProtocol.ConnectMsgHandlerUtil();

			foreach (var peer in peers)
			{
				try
				{
					var summary = util.GetDeviceSummary(peer.device_id);

					if (summary != null)
					{
						peer.Send(new
							{
								action = "backup-info",
								server_id = Settings.Default.ServerId,
								server_name = Environment.UserName,
								backup_startdate = summary.backup_range.start,
								backup_enddate = summary.backup_range.end,
								backup_folder = MyFileFolder.Photo,
								backup_folder_free_space = getStorageFreeSpace(),
								photo_count = summary.photo_count,
								video_count = summary.video_count,
								audio_count = summary.audio_count
							});
					}
				}
				catch (Exception e)
				{
					log4net.LogManager.GetLogger(this.GetType()).Warn("Unable to send backup info to peer:" + peer.device_name, e);
				}
			}
		}

		private static long getStorageFreeSpace()
		{
			try
			{
				var driveInfo = new DriveInfo(Path.GetPathRoot(MyFileFolder.Photo));
				var freeBytes = driveInfo.AvailableFreeSpace;
				return freeBytes;
			}
			catch (Exception e)
			{
				log4net.LogManager.GetLogger(typeof(ConnectionStatusUpdator)).Warn("Unable to get free space: " + MyFileFolder.Photo, e);
				return 0L;
			}
		}
	}
}
