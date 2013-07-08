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

		string GetUniqueDeviceFolder(string device_name);

		DeviceSummary GetDeviceSummary(string device_id);

		bool RejectUnpairedDevices { get; }
	}


}
