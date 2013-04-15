using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IConnectMsgHandlerUtil
	{
		ClientInfo GetClientInfo(string device_id);

		string GetServerId();

		string GetPhotoFolder();

		long GetFreeSpace(string path);

		void Save(ClientInfo clientInfo);
	}

	public class ClientInfo
	{
		public string device_id { get; set; }

		public string device_name { get; set; }

		public int photo_count { get; set; }

		public int video_count { get; set; }

		public int audio_count { get; set; }
	}
}
