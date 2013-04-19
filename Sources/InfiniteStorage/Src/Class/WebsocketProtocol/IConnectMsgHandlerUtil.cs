using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IConnectMsgHandlerUtil
	{
		Device GetClientInfo(string device_id);

		string GetServerId();

		string GetPhotoFolder();

		long GetFreeSpace(string path);

		void Save(Device clientInfo);

		//TimeRange GetBackupRange(string device_id);

		DeviceSummary GetDeviceSummary(string device_id);
	}

	
}
