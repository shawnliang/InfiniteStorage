using InfiniteStorage.Model;
using System;
using InfiniteStorage.Properties;

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
				if (Util.RejectUnpairedDevices)
				{
					ctx.SetState(new UnconnectedState());
					ctx.Send(new { action = "denied", reason = "Not allowed" });
					ctx.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "Not allowed");
				}
				else
				{
					ctx.Send(new { action = "wait-for-pair" });
					ctx.SetState(new WaitForApproveState());
					ctx.raiseOnPairingRequired();
				}
			}
			else
			{
				ctx.device_folder_name = clientInfo.folder_name;
				ReplyAcceptMsgToDevice(ctx, clientInfo);
			}
		}

		public void ReplyAcceptMsgToDevice(ProtocolContext ctx, Device clientInfo)
		{
			var summary = Util.GetDeviceSummary(clientInfo.device_id);

			var response = new
			{
				action = "accept",
				server_id = Util.GetServerId(),
				server_name = Settings.Default.LibraryName,
				backup_folder = Util.GetPhotoFolder(),
				backup_folder_free_space = Util.GetFreeSpace(Util.GetPhotoFolder()),

				backup_startdate = (summary != null) ? (DateTime?)summary.backup_range.start : null,
				backup_enddate = (summary != null) ? (DateTime?)summary.backup_range.end : null,

				photo_count = (summary != null) ? (long?)summary.photo_count : null,
				video_count = (summary != null) ? (long?)summary.video_count : null,
				audio_count = (summary != null) ? (long?)summary.audio_count : null
			};

			ctx.SetState(new TransmitInitState());
			ctx.raiseOnConnectAccepted();
			ctx.Send(response);
			ctx.storage.setDeviceName(ctx.device_folder_name);
		}
	}
}
