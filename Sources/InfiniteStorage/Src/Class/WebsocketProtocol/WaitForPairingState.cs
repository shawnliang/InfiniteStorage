using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteStorage.Model;
using Newtonsoft.Json;
namespace InfiniteStorage.WebsocketProtocol
{
	public class WaitForApproveState : AbstractProtocolState
	{
		public IConnectMsgHandlerUtil Util { get; set; }

		public WaitForApproveState()
		{
			this.Util = new ConnectMsgHandlerUtil();
		}

		public override void handleApprove(ProtocolContext ctx)
		{
			var dev = new Device
			{
				device_id = ctx.device_id,
				device_name = ctx.device_name,
			};

			Util.Save(dev);

			var response = new
				   {
					   action = "accept",
					   server_id = Util.GetServerId(),
					   backup_folder = Util.GetPhotoFolder(),
					   backup_folder_free_space = Util.GetFreeSpace(Util.GetPhotoFolder()),
					   photo_count = 0,
					   video_count = 0,
					   audio_count = 0
				   };
			try
			{
				ctx.SendText(JsonConvert.SerializeObject(response));
			}
			catch (Exception err)
			{
				throw new Exception("Unable to send ws cmd: Connection is closed?", err);
			}

			ctx.SetState(new TransmitInitState());
		}

		public override void handleDisapprove(ProtocolContext ctx)
		{
			var response = new
			{
				action = "denied",
				reason = "user rejected"
			};

			ctx.SendText(JsonConvert.SerializeObject(response));
			ctx.Stop(WebSocketSharp.Frame.CloseStatusCode.POLICY_VIOLATION, "User rejected");
			ctx.SetState(new UnconnectedState());


		}
	}
}
