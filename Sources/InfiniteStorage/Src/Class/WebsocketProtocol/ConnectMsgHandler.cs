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
				ctx.Send(new { action = "wait-for-pair" });
				ctx.raiseOnPairingRequired();
				return new WaitForApproveState();
			}
			else
			{
				ctx.device_folder_name = clientInfo.folder_name;
				return ReplyAcceptMsgToDevice(ctx, clientInfo);
			}
		}

		public AbstractProtocolState ReplyAcceptMsgToDevice(ProtocolContext ctx, Device clientInfo)
		{
			var summary = Util.GetDeviceSummary(clientInfo.device_id);

			var response = new
			{
				action = "accept",
				server_id = Util.GetServerId(),
				server_name = Environment.UserName,
				backup_folder = Util.GetPhotoFolder(),
				backup_folder_free_space = Util.GetFreeSpace(Util.GetPhotoFolder()),

				backup_startdate = (summary != null) ? (DateTime?)summary.backup_range.start : null,
				backup_enddate = (summary != null) ? (DateTime?)summary.backup_range.end : null,

				photo_count = (summary != null) ? (long?)summary.photo_count : null,
				video_count = (summary != null) ? (long?)summary.video_count : null,
				audio_count = (summary != null) ? (long?)summary.audio_count : null
			};

			ctx.raiseOnConnectAccepted();
			ctx.Send(response);
			ctx.storage.setDeviceName(ctx.device_folder_name);

			return new TransmitInitState();
		}
	}
}
