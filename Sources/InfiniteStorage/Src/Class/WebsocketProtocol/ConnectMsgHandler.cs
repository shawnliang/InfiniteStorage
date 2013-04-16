using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using InfiniteStorage.Model;

namespace InfiniteStorage.WebsocketProtocol
{
	public interface IConnectMsgHandler
	{
		void HandleConnectMsg(TextCommand cmd, ProtocolContext ctx);
	}

	public class ConnectMsgHandler : IConnectMsgHandler
	{
		public IConnectMsgHandlerUtil Util { get; set; }

		public ConnectMsgHandler()
		{
			this.Util = new ConnectMsgHandlerUtil();
		}

		public void HandleConnectMsg(TextCommand cmd, ProtocolContext ctx)
		{
			var clientInfo = Util.GetClientInfo(cmd.device_id);


			if (clientInfo == null)
			{
				clientInfo = new Device
				{
					device_name = cmd.device_name,
					device_id = cmd.device_id,
				};

				Util.Save(clientInfo);
			}

			var response = new
				{
					action = "accept",
					server_id = Util.GetServerId(),
					backup_folder = Util.GetPhotoFolder(),
					backup_folder_free_space = Util.GetFreeSpace(Util.GetPhotoFolder()),
					photo_count = clientInfo.photo_count,
					video_count = clientInfo.video_count,
					audio_count = clientInfo.audio_count
				};

			ctx.raiseOnConnectAccepted();
			ctx.SendText(JsonConvert.SerializeObject(response));
		}
	}
}
