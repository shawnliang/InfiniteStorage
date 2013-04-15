using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Properties;
using System.IO;

namespace InfiniteStorage.WebsocketProtocol
{
	class ConnectMsgHandlerUtil : IConnectMsgHandlerUtil
	{
		public ClientInfo GetClientInfo(string device_id)
		{
			throw new NotImplementedException();
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

		public void Save(ClientInfo clientInfo)
		{
			throw new NotImplementedException();
		}
	}
}
