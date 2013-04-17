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
		AbstractProtocolState HandleConnectMsg(TextCommand cmd, ProtocolContext ctx);
	}

	public class ConnectMsgHandler : IConnectMsgHandler
	{
		public IConnectMsgHandlerUtil Util { get; set; }

		public ConnectMsgHandler()
		{
			this.Util = new ConnectMsgHandlerUtil();
		}

		public AbstractProtocolState HandleConnectMsg(TextCommand cmd, ProtocolContext ctx)
		{
			var clientInfo = Util.GetClientInfo(cmd.device_id);

			if (clientInfo == null)
			{
				ctx.SendText(JsonConvert.SerializeObject(new { action = "wait-for-pair" }));
				ctx.raiseOnPairingRequired();
				return new WaitForApproveState();
			}
			else
			{
				return ReplyAcceptMsgToDevice(ctx, clientInfo);
			}
		}

		public AbstractProtocolState ReplyAcceptMsgToDevice(ProtocolContext ctx, Device clientInfo)
		{
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
			ctx.storage.setDeviceName(ctx.device_name);

			return new TransmitInitState();
		}
	}
}
